$(function () {
	window.lookups = getTripLookups();

	$('#addTripButton').click(function () {
		initiateAddTripModel(window.lookups, null, null);
	});
	
	wireTripEditModalEvents();
	initWorkingObject
		(
			{
				'addDeleteToTripRow': false
			}
		);

	//load todays trips
	$.ajax({
		type: 'POST',
		dataType: 'json',
		url: '/Trips/GetAddedTodayTrips',
		success: function (data) {
			$('#loadingDiv').hide();
			if (data.length > 0) {
				showTrips(data);
			}
		},
		error: function (jqXHR, textStatus, errorThrown) { alert(errorThrown); }
	});

	function showTrips(tripsArray) {
		$('#tripsTable').show();
		$('#tripsTable #tripsTable tbody').empty();

		var tripRow = "";
		$.each(tripsArray, function (index, trip) {
			tripRow = constructTripRow(trip);
			$('#tripsTable tbody').append($(tripRow));
		});

		resetTripsTableCounter($('#tripsTable'));
	}
});