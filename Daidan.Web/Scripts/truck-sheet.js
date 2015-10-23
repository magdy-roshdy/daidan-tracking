$(function () {
	$('#viewSheetButton').click(function () {
		var isValid = validateField($('#year'), /^\d+$/, 'Please enter the year', '#searchPanel');

		if (isValid) {
			$('#loadingDiv').show();
			$('#viewReportButton').attr('disabled', 'disabled');
			$('#searchResult').hide();

			var parameters = { 'TruckId': $('#trucks').val(), 'Month': $('#months').val(), 'Year': $('#year').val() }
			$.ajax({
				type: 'POST',
				dataType: 'json',
				url: '/Reports/TruckSheetSearch',
				data: parameters,
				success: function (data) {
					$('#loadingDiv').hide();
					$('#viewReportButton').removeAttr('disabled');

					showSearchResult(data)
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
		$('#searchResult #driversSalaryPortionsTable tbody').empty();

		var counter = 1;
		var tripRow = "";
		var costSum = 0;
		var priceSum = 0;
		var profitSum = 0;
		$.each(sheetInfo.Trips, function (index, trip) {
			tripRow = constructTripRowForTruckSheet(counter, trip);
			$('#searchResult #resultTable tbody').append($(tripRow));
			counter++;

			costSum += trip.TripTotalCost;
			priceSum += trip.TripTotalPrice;
			profitSum += trip.TripNetProfit;
		});

		//add sum row
		tripRow = constructTripRowForTruckSheet('', { 'TripTotalCost': costSum, 'TripTotalPrice': priceSum, 'TripNetProfit': profitSum, 'AdminFeesAmount': sheetInfo.MonthAdminFeesSum });
		$('#searchResult #resultTable tbody').append($(tripRow).addClass('table-header-row'));


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
			$('#searchResult #expensesTable tbody').append($(expenseRow).addClass('table-header-row'));
		}

		//drivers salaries table
		$.each(sheetInfo.DriversSalaryPortion, function (index, driverSalary) {
			expenseRow = constructDriverSalaryRow(driverSalary);
			$('#searchResult #driversSalaryPortionsTable tbody').append($(expenseRow));
		});

		if (sheetInfo.DriversSalaryPortion.length > 1) {
			expenseRow = constructDriverSalaryRow(
				{
					'Driver': { 'Name': '' },
					'NumberOfTrips': sheetInfo.DriversSalaryPortionTripsCount,
					'PortionAmount': sheetInfo.DriversSalaryPortionSum
				}
			);
			$('#searchResult #driversSalaryPortionsTable tbody').append($(expenseRow).addClass('table-header-row'));
		}

		//final values
		$('#searchResult #monthFinalValuesTable #monthTripsProfitCell').text(profitSum.toFixed(2));
		$('#searchResult #monthFinalValuesTable #monthTripsCostCell').text((expenseSum + sheetInfo.DriversSalaryPortionSum).toFixed(2));
		$('#searchResult #monthFinalValuesTable #monthFinalValueCell').text((profitSum - expenseSum - sheetInfo.DriversSalaryPortionSum).toFixed(2));
	}

	function constructTripRowForTruckSheet(counter, tripObject) {
		var zero = 0;
		var newTripRow = "<tr>\
			<td style='text-align: center;'>" + counter.toString() + "</td>\
			<td style='text-align: center;'>" + (tripObject.Date ? getDateFromJSON(tripObject.Date).format('DD/MM/YYYY') : '') + "</td>\
			<td style='text-align: center;'>" + (tripObject.Site ? tripObject.Site.Customer.Name : '') + "</td>\
			<td style='text-align: center;'>" + (tripObject.Site ? tripObject.Site.Name : '') + "</td>\
			<td style='text-align: center;'>" + (tripObject.Material ? tripObject.Material.Name : '') + "</td>\
			<td style='text-align: center;'>" + (tripObject.QuantityCaption ? tripObject.QuantityCaption : '') + "</td>\
			<td style='text-align: center;'>" + tripObject.TripTotalCost.toFixed(2) + "</td>\
			<td style='text-align: center;'>" + tripObject.TripTotalPrice.toFixed(2) + "</td>\
			<td style='text-align: center;'>" + (tripObject.TripNetProfit ? tripObject.TripNetProfit.toFixed(2) : zero.toFixed(2)) + "</td>\
			<td style='text-align: center;' class='hide-print'>" + (tripObject.AdminFeesCaption ? tripObject.AdminFeesCaption : '') + "</td>\
			<td style='text-align: center;' class='hide-print'>" + (tripObject.AdminFeesAmount ? tripObject.AdminFeesAmount.toFixed(2) : '') + "</td>\
		</tr>";

		return newTripRow;
	}

	function constructTruckExpenseRow(expenseObject) {
		return "<tr style='height: 27px;'>\
			<td style='text-align: left; padding-left: 10px;'><strong>" + expenseObject.Section.Name + "</strong></td>\
			<td style='text-align: center;width: 100px;'>" + expenseObject.Amount.toFixed(2) + "</td>\
		</tr>";
	}

	function constructDriverSalaryRow(driverSalaryObject) {
		return "<tr>\
			<td style='text-align: center;'>" + driverSalaryObject.Driver.Name + "</td>\
			<td style='text-align: center;'>" + driverSalaryObject.NumberOfTrips + "</td>\
			<td style='text-align: center;'>" + driverSalaryObject.PortionAmount.toFixed(2) + "</td>\
		</tr>";
	}

	$('#printButton').click(function () {
		$('.print-area').printArea();
	});

	$('#excelExportButton').click(function () {
		var $printAreaClone = $('.print-area').clone();
		$('.hide-print', $printAreaClone).remove();
		HTMLToExcel($printAreaClone[0], 'Summary Sheet');
	});

	//check auto load querystrings
	if ($.QueryString["month"] && $.QueryString["year"] && $.QueryString["truckId"]) {
		$('#trucks').val($.QueryString["truckId"])
		$('#months').val($.QueryString["month"]);
		$('#year').val($.QueryString["year"]);

		$('#viewSheetButton').click();
	}
});

