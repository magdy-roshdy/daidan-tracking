$(function () {
	window.lookups = getTripLookups();


	fillDropDown($('#drivers'), window.lookups.Drivers, 'Name');
	fillDropDown($('#trucks'), window.lookups.Trucks, 'Number');
	fillDropDown($('#customers'), window.lookups.Customers, 'Name');
	fillDropDown($('#materials'), window.lookups.Materials, 'Name');
	fillDropDown($('#units'), window.lookups.Units, 'Name');
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

	//---
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

	$('#searchButton').click(function () {
		$(this).attr('disabled', 'disabled');
		$('#loadingDiv').show();
		$('#searchResult').hide();
		$('#noResultMessage').hide();

		$('#updatePONumberButton').attr('disabled', 'disabled');
		$('#updateSellingPriceButton').attr('disabled', 'disabled');

		var parameters = {
			'VoucherNumber': $('#voucherNumber').val(),
			'PONumber': $('#PONumber').val(),
			'TicketNumber': $('#ticketNumber').val(),
			'From': moment($('#fromDate').val(), "DD/MM/YYYY").format('MM/DD/YYYY'),
			'To': moment($('#toDate').val(), "DD/MM/YYYY").format('MM/DD/YYYY'),
			'DriverId': $('#drivers').val(),
			'TruckId': $('#trucks').val(),
			'CustomerId': $('#customers').val(),
			'SiteId': $('#sites').val(),
			'MaterialId': $('#materials').val(),
			'UnitId': $('#units').val()
		};
		$.ajax({
			type: 'POST',
			dataType: 'json',
			url: '/Reports/MasterReportSearch',
			data: parameters,
			success: function (data) {
				$('#loadingDiv').hide();
				if (data.length > 0) {
					showSearchResult(data);
				} else {
					$('#noResultMessage').show();
				}
				
				$('#searchButton').removeAttr('disabled');
			},
			error: function (jqXHR, textStatus, errorThrown) { alert(errorThrown); }
		});
	});

	wireTripEditModalEvents();
	initWorkingObject
	(
		{
			'addDeleteToTripRow': true,
			'addCheckboxToTripRow': true
		}
	);

	//--------
	$('#searchResult #resultTable thead #checkAll').click(function () {
		var self = this;
		$('#searchResult #resultTable tbody input:checkbox').each(function (index, checkBox) {
			checkBox.checked = self.checked;
			if (self.checked) {
				$('#updatePONumberButton').removeAttr('disabled');
				$('#updateSellingPriceButton').removeAttr('disabled');
			} else {
				$('#updatePONumberButton').attr('disabled', 'disabled');
				$('#updateSellingPriceButton').attr('disabled', 'disabled');
			}
		});
	});

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
		var valid = validateField($('#updatePONumberModal #poNumber'), /^\d+$/, "Please enter P.O number", '#updatePONumberModal');
		if (valid) {
			var idsArray = [];
			$('#searchResult #resultTable tbody input:checkbox').each(function (index, checkBox) {
				if (checkBox.checked) {
					idsArray.push(parseInt($(checkBox).attr('data-tripId')));
				}
			});

			if (confirm('Are you sure you want to update P.O number for ' + idsArray.length.toString() + ' trip(s)')) {

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
						$('#searchButton').click(); //update result
						$('#searchResult #resultTable thead #checkAll')[0].checked = false; //uncheck it
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
		var modalObj =  $('#updateSellingPriceModal');
		var textBoxObj = $('#updateSellingPriceModal #sellingPrice');
		var valid = validateField(textBoxObj, /^(?:\d*\.\d{1,2}|\d+)$/, "Please enter selling price", '#updateSellingPriceModal');
		if (valid) {
			var idsArray = [];
			$('#searchResult #resultTable tbody input:checkbox').each(function (index, checkBox) {
				if (checkBox.checked) {
					idsArray.push(parseInt($(checkBox).attr('data-tripId')));
				}
			});

			if (confirm('Are you sure you want to update selling price for ' + idsArray.length.toString() + ' trip(s)')) {

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
						$('#searchResult #resultTable thead #checkAll')[0].checked = false; //uncheck it
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

	var tripRow = "";
	$.each(tripsArray, function (index, trip) {
		tripRow = constructTripRow(trip);
		$('#searchResult #resultTable tbody').append($(tripRow));
	});

	//attach checkbox events
	$('#searchResult #resultTable tbody input:checkbox').click(function () {
		if (this.checked) {
			$('#updatePONumberButton').removeAttr('disabled');
			$('#updateSellingPriceButton').removeAttr('disabled');
		} else {
			//check if any other checkboxes are checked
			var _checked = false;
			$('#searchResult #resultTable tbody input:checkbox').each(function (index, checkBox) {
				if (checkBox.checked) {
					_checked = true;
				}
			});

			if (!_checked) {
				$('#updatePONumberButton').attr('disabled', 'disabled');
				$('#updateSellingPriceButton').attr('disabled', 'disabled');
			}

			$('#searchResult #resultTable thead #checkAll')[0].checked = false;
		}
	});
}

function deleteTrip(deleteDiv) {
	if (confirm('Trip will be deleted permanently, are you sure?!')) {
		window.currentWorkObject.deletedTD = $(deleteDiv).parent();

		$.ajax({
			type: 'POST',
			dataType: 'json',
			url: '/Trips/DeletedTrip',
			data: { tripId: $('input', $(deleteDiv).parent()).val() },
			success: function (data) {
				window.currentWorkObject.deletedTD.parent().remove();
				initWorkingObject
				(
					{
						'addDeleteToTripRow': true,
						'addCheckboxToTripRow': true
					}
				);

				showNotification('Trip deleted succesfully <i class="fa fa-check fa-lg"></i>', 'success');
			},
			error: function (jqXHR, textStatus, errorThrown) { alert(errorThrown); }
		});
	}
}