$(function () {
	window.lookups = getTripLookups();
	fillDropDown($('#customers'), window.lookups.Customers, 'Name');
	fillDropDown($('#materials'), window.lookups.Materials, 'Name');
	$('#fromDateGroup').datepicker(
		{
			autoclose: true,
			format: 'dd/mm/yyyy',
			orientation: 'top left'
		}
	);
	$('#toDateGroup').datepicker(
		{
			autoclose: true,
			format: 'dd/mm/yyyy',
			orientation: 'top left'
		}
	);

	$('#customers').change(function () {
		var customerSites = $.grep(window.lookups.Sites, function (site, index) {
			return site.Customer.Id == parseInt($('#customers').val());
		});

		var sitesSelect = $('#sites').empty();
		sitesSelect.append($("<option />").val('').text('[ALL]'));
		$.each(customerSites, function (index, site) {
			sitesSelect.append($("<option />").val(site.Id).text(site.Name));
		});
	});

	$('#printButton').click(function () {
		$('.print-area').printArea();
	});

	$('#excelExportButton').click(function () {
		var $printAreaClone = $('.print-area').clone();
		$('.hide-print', $printAreaClone).remove();
		HTMLToExcel($printAreaClone[0], 'Summary Sheet');
	});

	$('#viewReportButton').click(function () {
		if(validateReportForm()){
			$(this).attr('disabled', 'disabled');
			$('#loadingDiv').show();
			$('#searchResult').hide();
			$('#noResultMessage').hide();

			var parameters = {
				'PONumber': $('#PONumber').val(),
				'From': moment($('#fromDate').val(), "DD/MM/YYYY").format('DD/MM/YYYY'),
				'To': moment($('#toDate').val(), "DD/MM/YYYY").format('DD/MM/YYYY'),
				'CustomerId': $('#customers').val(),
				'SiteId': $('#sites').val(),
				'MaterialId': $('#materials').val()
			};

			$.ajax({
				type: 'POST',
				dataType: 'json',
				url: '/Trips/MasterSearch',
				data: parameters,
				success: function (data) {
					$('#loadingDiv').hide();
					if (data.length > 0) {
						showSearchResult(data)
					} else {
						$('#noResultMessage').show();
					}

					$('#viewReportButton').removeAttr('disabled');
				},
				error: function (jqXHR, textStatus, errorThrown) { alert(errorThrown); }
			});
		}
	});

	//---
	$('#updatePONumberButton').click(function () {
		$('#updatePONumberModal #poNumber').val('');
		$('#updatePONumberModal').modal().on('shown.bs.modal',
			function () {
				$('#updatePONumberModal #poNumber').focus();
			}
		);
	});

	$('#updateSellingPriceButton').click(function () {
		$('#updateSellingPriceModal #sellingPrice').val('');
		$('#updateSellingPriceModal').modal().on('shown.bs.modal',
			function () {
				$('#updateSellingPriceModal #sellingPrice').focus();
			}
		);
	});

	$('#updatePONumberModal .btn-primary').click(function () {
		var valid = validateField($('#updatePONumberModal #poNumber'), null, "Please enter P.O number", '#updatePONumberModal');
		if (valid) {
			var idsArray = [];
			$('#searchResult #resultTable tbody input:hidden').each(function (index, hidden) {
				idsArray.push(parseInt($(hidden).val()));
			});

			if (confirm('Are you sure you want to update P.O number for all the trips?')) {
				$('#updatePONumberModal .btn-primary').attr('disabled', 'disabled');

				$.ajax({
					type: 'POST',
					dataType: 'json',
					url: '/Trips/PONumberBatchUpdate',
					data: { 'TripsIds': idsArray, 'PONumber': $('#updatePONumberModal #poNumber').val() },
					success: function (data) {
						$('#updatePONumberModal .btn-primary').removeAttr('disabled');

						alert("Update Done Successfully!");
						$('#updatePONumberModal').modal('hide');
						$('#viewReportButton').click(); //update result
					},
					error: function (jqXHR, textStatus, errorThrown) {
						alert(errorThrown);
						$('#updatePONumberModal .btn-primary').removeAttr('disabled');
						$('#updatePONumberModal').modal('hide');
					}
				});
			} else {
				$('#updatePONumberModal').modal('hide');
			}
		}
	});

	$('#updateSellingPriceModal .btn-primary').click(function () {
		var modalObj = $('#updateSellingPriceModal');
		var textBoxObj = $('#updateSellingPriceModal #sellingPrice');
		var valid = validateField(textBoxObj, /^(?:\d*\.\d{1,3}|\d+)$/, "Please enter selling price", '#updateSellingPriceModal');
		if (valid) {
			var idsArray = [];
			$('#searchResult #resultTable tbody input:hidden').each(function (index, hidden) {
				idsArray.push(parseInt($(hidden).val()));
			});

			if (confirm('Are you sure you want to update selling price for all trips?')) {
				$(this).attr('disabled', 'disabled');

				$.ajax({
					type: 'POST',
					dataType: 'json',
					url: '/Trips/SellingPriceBatchUpdate',
					data: { 'TripsIds': idsArray, 'SellingPrice': parseFloat($('#updateSellingPriceModal #sellingPrice').val()) },
					success: function (data) {
						$('.btn-primary', modalObj).removeAttr('disabled');

						alert("Update Done Successfully!");
						modalObj.modal('hide');
						$('#viewReportButton').click(); //update result
					},
					error: function (jqXHR, textStatus, errorThrown) {
						alert(errorThrown);
						$('.btn-primary', modalObj).removeAttr('disabled');
						modalObj.modal('hide');
					}
				});
			} else {
				modalObj.modal('hide');
			}
		}
	});
});

function showSearchResult(tripsArray) {
	$('#searchResult').show();
	$('#searchResult #resultTable tbody tr').remove();

	//
	$('#searchResult #customerNameHead').text($('#customers').children(':selected').text());
	$('#searchResult #siteCell').text($('#sites').children(':selected').text());
	$('#searchResult #materialCell').text($('#materials').children(':selected').text());
	$('#searchResult #fromCell').text($('#fromDate').val());
	$('#searchResult #toCell').text($('#toDate').val());

	var tripRow = "";
	var counter = 1;
	var quantitySum = 0;
	var tripPriceSum = 0;
	var tripsUnit = '';
	$.each(tripsArray, function (index, trip) {
		trip.QuantityCaption = trip.UnitsQuantity.toFixed(2);
		tripRow = constructTripRowForCustomerReport(counter, trip);
		$('#searchResult #resultTable tbody').append($(tripRow));
		counter++;

		quantitySum += trip.UnitsQuantity;
		tripPriceSum += trip.TripTotalPrice;
		tripsUnit = trip.Unit.Name;
	});

	//sum row
	var sumTripObj = { 'QuantityCaption': quantitySum.toFixed(2) + ' ' + tripsUnit, 'TripTotalPrice': tripPriceSum, 'Id': 0 };
	var totalRowObj = $(constructTripRowForCustomerReport('', sumTripObj, true));
	totalRowObj.css('font-weight', 'bold');
	totalRowObj.css('height', '35px');
	totalRowObj.css('background-color', '#EBEBEB');
	$('#searchResult #resultTable tbody').append(totalRowObj);
}

function validateReportForm() {
	var isValid = true;

	var customers = $('#customers');
	isValid = validateField(customers, null, 'Please select the customer', '#searchPanel');
	
	if (isValid) {
		var sites = $('#sites');
		isValid = validateField(sites, null, 'Please select the site', '#searchPanel');
	}

	if (isValid) {
		var fromDate = $('#fromDate');
		isValid = validateField(fromDate, null, 'Please enter the starting date', '#searchPanel');
	}

	if (isValid) {
		var toDate = $('#toDate');
		isValid = validateField(toDate, null, 'Please enter the end date', '#searchPanel');
	}

	if (isValid) {
		var materials = $('#materials');
		isValid = validateField(materials, null, 'Please select the material', '#searchPanel');
	}

	if (isValid) {
		var poNumber = $('#PONumber');
		if (poNumber.val()) {
			isValid = validateField(poNumber, /^\d+$/, 'Please enter a valid P.O number', '#searchPanel');
		}
	}

	return isValid;
}

function constructTripRowForCustomerReport(counter, tripObject, hideDelete) {
	var newTripRow = "<tr>\
		<td style='text-align: center;' class='counterCell'>" + counter.toString() + "</td>\
		<td style='text-align: center;'>" + (tripObject.Date ? getDateFromJSON(tripObject.Date).format('DD/MM/YYYY') : '') + "</td>\
		<td style='text-align: center;'>" + (tripObject.VoucherNumber ? tripObject.VoucherNumber : '') + "</td>\
		<td style='text-align: center;'>" + (tripObject.PONumber ? tripObject.PONumber : '') + "</td>\
		<td style='text-align: center;'>" + (tripObject.Truck ? tripObject.Truck.Number : '') + "</td>\
		<td style='text-align: center;'>" + (tripObject.QuantityCaption ? tripObject.QuantityCaption : '') + "</td>\
		<td style='text-align: center;'>" + (tripObject.UnitSellingPrice ? tripObject.UnitSellingPrice.toFixed(3) : '') + "</td>\
		<td style='text-align: center;'>" + (tripObject.TripTotalPrice ? tripObject.TripTotalPrice.toFixed(2) : '') + "</td>\
		<td style='text-align: center;' class='hide-print'>\
		<input type='hidden' value='" + tripObject.Id.toString() + "' />";
	if (!hideDelete) {
		newTripRow += "<div style='cursor: pointer;' onclick='removeTripFromReport(this);'><img src='/Content/images/delete.png' alt='' /></div>";
	} else
	{
		newTripRow += "&nbsp;"
	}
	newTripRow += "</td></tr>";

	return newTripRow;
}

function removeTripFromReport(deleteDiv) {
	if (confirm('Are you sure?')) {
		$(deleteDiv).parent().parent().remove();
	}

	resetCustomerTripsTableCounter($('#resultTable'));
}

function resetCustomerTripsTableCounter(tableObject) {
	var cells = $('td.counterCell', tableObject[0]);
	var counter = 1;
	$.each(cells, function (index, cell) {
		if($(cell).text())
			$(cell).text(counter++);
	});
}