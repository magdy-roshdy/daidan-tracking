$(function () {
	window.lookups = getTripLookups();

	$('#addTripButton').click(function () {
		initiateAddTripModel(window.lookups, null, null);
	});
	
	wireTripEditModalEvents();
	initWorkingObject({ addDeleteToTripRow: false });
});