$(function () {
	$('#viewSheetButton').click(function () {
		var isValid = validateField($('#year'), /^\d+$/, 'Please enter the year', '#searchPanel');

		var parameters = { 'TruckId': $('#trucks').val(), 'Month': $('#months').val(), 'Year': $('#year').val() }
		if (isValid) {
			$('#loadingDiv').show();
			$('#viewReportButton').attr('disabled', 'disabled');
			$('#searchResult').hide();


			$.ajax({
				type: 'POST',
				dataType: 'json',
				url: '/Reports/TruckSheetSearch',
				data: parameters,
				success: function (data) {
					$('#loadingDiv').hide();
					$('#viewReportButton').removeAttr('disabled');

					showSearchResult(data)
					
					console.log(data);
				},
				error: function (jqXHR, textStatus, errorThrown) { alert(errorThrown); }
			});
		}
	});

	function showSearchResult(sheetInfo) {
		$('#searchResult').show();

		$('#searchResult #truckNumberCell').text($('#trucks').children(':selected').text());
		$('#searchResult #monthCell').text($('#months').children(':selected').text() + ' ' + $('#year').val());
		$('#searchResult #expensesTable tbody').empty();
		$('#searchResult #resultTable tbody').empty();

		var counter = 1;
		var tripRow = "";
		$.each(sheetInfo.Trips, function (index, trip) {
			tripRow = constructTripRowForTruckSheet(counter, trip);
			$('#searchResult #resultTable tbody').append($(tripRow));
			counter++;
		});

		

		var expenseRow = "";
		var expenseSum = 0;
		$.each(sheetInfo.Expenses, function (index, expense) {
			expenseRow = constructTruckExpenseRow(expense);
			$('#searchResult #expensesTable tbody').append($(expenseRow));

			expenseSum += expense.Amount;
		});

		if (expenseSum > 0)
		{
			expenseRow = constructTruckExpenseRow({ 'Section': { 'Name': '' }, 'Amount': expenseSum });
			$('#searchResult #expensesTable tbody').append($(expenseRow));
		}
	}

	function constructTripRowForTruckSheet(counter, tripObject) {
		var newTripRow = "<tr>\
			<td style='text-align: center;'>" + counter.toString() + "</td>\
			<td style='text-align: center;'>" + (tripObject.Date ? getDateFromJSON(tripObject.Date).format('DD/MM/YYYY') : '') + "</td>\
			<td style='text-align: center;'>" + tripObject.Site.Customer.Name + "</td>\
			<td style='text-align: center;'>" + tripObject.Site.Name+ "</td>\
			<td style='text-align: center;'>" + tripObject.Material.Name + "</td>\
			<td style='text-align: center;'>" + tripObject.QuantityCaption + "</td>\
			<td style='text-align: center;'>" + tripObject.UnitCost.toFixed(2) + "</td>\
			<td style='text-align: center;'>" + tripObject.TripTotalCost.toFixed(2) + "</td>\
			<td style='text-align: center;'>" + (tripObject.UnitSellingPrice ? tripObject.UnitSellingPrice.toFixed(2) : '') + "</td>\
			<td style='text-align: center;'>" + (tripObject.TripTotalPrice ? tripObject.TripTotalPrice.toFixed(2) : '') + "</td>\
		</tr>";

		return newTripRow;
	}

	function constructTruckExpenseRow(expenseObject) {
		var newTripRow = "<tr>\
			<td style='text-align: left;'><strong>" + expenseObject.Section.Name + "</strong></td>\
			<td style='text-align: center;width: 100px;'>" + expenseObject.Amount.toFixed(2) + "</td>\
		</tr>";

		return newTripRow;
	}

	$('#printButton').click(function () {
		$('.print-area').printArea();
	});
});

