$(function () {
	window.lookups = getTripLookups();
	initWorkingObject
		(
			{
				'addDeleteToTripRow': false,
				'addCheckboxToTripRow': false
			}
		);

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

	

	$('#viewReportButton').click(function () {
		if(validateReportForm()){
			$(this).attr('disabled', 'disabled');
			$('#loadingDiv').show();
			$('#searchResult').hide();
			$('#noResultMessage').hide();

			var parameters = {
				'PONumber': $('#PONumber').val(),
				'From': moment($('#fromDate').val(), "DD/MM/YYYY").format('MM/DD/YYYY'),
				'To': moment($('#toDate').val(), "DD/MM/YYYY").format('MM/DD/YYYY'),
				'CustomerId': $('#customers').val(),
				'SiteId': $('#sites').val(),
				'MaterialId': $('#materials').val()
			};

			$.ajax({
				type: 'POST',
				dataType: 'json',
				url: '/Reports/MasterReportSearch',
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
	$.each(tripsArray, function (index, trip) {
		tripRow = constructTripRowForCustomerReport(counter, trip);
		$('#searchResult #resultTable tbody').append($(tripRow));
		counter++;

		quantitySum += trip.UnitsQuantity;
		tripPriceSum += trip.TripTotalPrice;
	});

	//sum row
	var sumTripObj = { 'QuantityCaption': quantitySum.toFixed(2), 'TripTotalPrice': tripPriceSum };
	tripRow = constructTripRowForCustomerReport('', sumTripObj);
	$('#searchResult #resultTable tbody').append($(tripRow));
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

function constructTripRowForCustomerReport(counter, tripObject) {
	var newTripRow = "<tr>\
		<td style='text-align: center;'>" + counter.toString() + "</td>\
		<td style='text-align: center;'>" + (tripObject.Date ? getDateFromJSON(tripObject.Date).format('DD/MM/YYYY') : '') + "</td>\
		<td style='text-align: center;'>" + (tripObject.VoucherNumber ? tripObject.VoucherNumber : '') + "</td>\
		<td style='text-align: center;'>" + (tripObject.PONumber ? tripObject.PONumber : '') + "</td>\
		<td style='text-align: center;'>" + (tripObject.Truck ? tripObject.Truck.Number : '') + "</td>\
		<td style='text-align: center;'>" + (tripObject.QuantityCaption ? tripObject.QuantityCaption : '') + "</td>\
		<td style='text-align: center;'>" + (tripObject.UnitSellingPrice ? tripObject.UnitSellingPrice.toFixed(2) : '') + "</td>\
		<td style='text-align: center;'>" + (tripObject.TripTotalPrice ? tripObject.TripTotalPrice.toFixed(2) : '') + "</td>\
	</tr>";

	return newTripRow;
}
