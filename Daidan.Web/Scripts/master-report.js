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
			'From': moment($('#fromDate').val(), "DD/MM/YYYY").format('DD/MM/YYYY'),
			'To': moment($('#toDate').val(), "DD/MM/YYYY").format('DD/MM/YYYY'),
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
			'addDeleteToTripRow': true
		}
	);
});

function showSearchResult(tripsArray) {
	$('#searchResult').show();
	$('#searchResult #resultTable tbody tr').remove();

	var tripRow = "";
	$.each(tripsArray, function (index, trip) {
		tripRow = constructTripRow(trip);
		$('#searchResult #resultTable tbody').append($(tripRow));
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
						'addDeleteToTripRow': true
					}
				);

				showNotification('Trip deleted succesfully <i class="fa fa-check fa-lg"></i>', 'success');
			},
			error: function (jqXHR, textStatus, errorThrown) { alert(errorThrown); }
		});
	}
}