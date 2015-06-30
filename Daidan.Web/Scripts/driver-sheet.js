$(function () { 
	$('#viewSheetButton').click(function () {
		var isValid = validateField($('#year'), /^\d+$/, 'Please enter the year', '#searchPanel');

		if (isValid) {
			$('#loadingDiv').show();
			$('#viewReportButton').attr('disabled', 'disabled');
			$('#searchResult').hide();

			var parameters = { 'DriverId': $('#drivers').val(), 'Month': $('#months').val(), 'Year': $('#year').val() }
			$.ajax({
				type: 'POST',
				dataType: 'json',
				url: '/Reports/DriverSheetSearch',
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

		$('#searchResult #driverNameCell').text($('#drivers').children(':selected').text());
		$('#searchResult #monthCell').text($('#months').children(':selected').text() + ' ' + $('#year').val());
		$('#searchResult #expensesTable tbody').empty();
		$('#searchResult #resultTable tbody').empty();

		$('#searchResult #summaryTable #previousMonthBalanceCell_').empty();
		$('#searchResult #summaryTable #previousMonthBalanceCell').empty();
		$('#searchResult #summaryTable #salaryCell').empty();
		$('#searchResult #summaryTable #tripProfit').empty();
		$('#searchResult #summaryTable #recievedCash').empty();
		$('#searchResult #summaryTable #paidCash').empty();
		$('#searchResult #summaryTable #paidExpenses').empty();
		$('#searchResult #summaryTable #monthBalanaceCell_').empty();
		$('#searchResult #summaryTable #monthBalanaceCell').empty();
		$('#searchResult #recievedCashTable tbody').empty();
		

		var counter = 1;
		var tripRow = "";
		var costSum = 0;
		var extraCostSum = 0;
		var profitSum = 0;
		$.each(sheetInfo.Trips, function (index, trip) {
			tripRow = constructTripRowForTruckSheet(counter, trip);
			$('#searchResult #resultTable tbody').append($(tripRow));
			counter++;

			costSum += trip.TripTotalCost;
			profitSum += trip.TripNetProfit;
			extraCostSum += trip.ExtraCost;
		});

		//add sum row
		tripRow = constructTripRowForTruckSheet('', { 'TripTotalCost': costSum, 'ExtraCost': extraCostSum, 'TripNetProfit': profitSum });
		$('#searchResult #resultTable tbody').append($(tripRow).css('font-weight', 'bold').css('height', '35px').css('background-color', '#EBEBEB'));

		//fill summary info
		if(sheetInfo.PreviousMonthBalanace < 0)
			$('#searchResult #summaryTable #previousMonthBalanceCell_').text((sheetInfo.PreviousMonthBalanace * -1).toFixed(2));
		else
			$('#searchResult #summaryTable #previousMonthBalanceCell').text(sheetInfo.PreviousMonthBalanace.toFixed(2));

		$('#searchResult #summaryTable #salaryCell').text(sheetInfo.MonthSalary.toFixed(2));
		$('#searchResult #summaryTable #tripProfit').text(sheetInfo.DriverTripsProfit.toFixed(2));
		$('#searchResult #summaryTable #recievedCash').text(sheetInfo.MonthDriverCashSum.toFixed(2));
		$('#searchResult #summaryTable #paidCash').text(sheetInfo.ExtraCostSum.toFixed(2));
		$('#searchResult #summaryTable #paidExpenses').text(sheetInfo.MonthPaidExpensesSum.toFixed(2));

		if (sheetInfo.MonthBalance < 0)
			$('#searchResult #summaryTable #monthBalanaceCell_').text((sheetInfo.MonthBalance * -1).toFixed(2));
		else
			$('#searchResult #summaryTable #monthBalanaceCell').text(sheetInfo.MonthBalance.toFixed(2));

		//month recieved cash list
		counter = 1;
		var cashSum = 0;
		var cashRow = "";
		$.each(sheetInfo.MonthDriverCash, function (index, cash) {
			cashRow = constructRecievedCashRow(counter, cash);
			$('#searchResult #recievedCashTable tbody').append($(cashRow));
			counter++;

			cashSum += cash.Amount;
		});

		if (cashSum > 0){
			cashRow = constructRecievedCashRow('', { 'VoucherNumber': '', 'Amount': cashSum });
			$('#searchResult #recievedCashTable tbody').append($(cashRow).css('font-weight', 'bold').css('height', '35px').css('background-color', '#EBEBEB'));
		}
	}

	function constructTripRowForTruckSheet(counter, tripObject) {
		var zero = 0;
		var newTripRow = "<tr>\
			<td style='text-align: center;'>" + counter.toString() + "</td>\
			<td style='text-align: center;'>" + (tripObject.Date ? getDateFromJSON(tripObject.Date).format('DD/MM/YYYY') : '') + "</td>\
			<td style='text-align: center;'>" + (tripObject.VoucherNumber ? tripObject.VoucherNumber : '' ) + "</td>\
			<td style='text-align: center;'>" + (tripObject.Truck ? tripObject.Truck.Number : '') + "</td>\
			<td style='text-align: center;'>" + (tripObject.Site ? tripObject.Site.Customer.Name : '') + "</td>\
			<td style='text-align: center;'>" + (tripObject.Site ? tripObject.Site.Name : '') + "</td>\
			<td style='text-align: center;'>" + (tripObject.Material ? tripObject.Material.Name : '') + "</td>\
			<td style='text-align: center;'>" + (tripObject.QuantityCaption ? tripObject.QuantityCaption : '') + "</td>\
			<td style='text-align: center;'>" + tripObject.TripTotalCost.toFixed(2) + "</td>\
			<td style='text-align: center;'>" + tripObject.ExtraCost.toFixed(2) + "</td>\
			<td style='text-align: center;'>" + (tripObject.TripNetProfit ? tripObject.TripNetProfit.toFixed(2) : zero.toFixed(2)) + "</td>\
		</tr>";

		return newTripRow;
	}

	function constructRecievedCashRow(counter, cashObject) {
		var cashRow = "<tr>\
			<td style='text-align: center;'>" + counter.toString() + "</td>\
			<td style='text-align: center;'>" + (cashObject.Date ? getDateFromJSON(cashObject.Date).format('DD/MM/YYYY') : '') + "</td>\
			<td style='text-align: center;'>" + cashObject.VoucherNumber+ "</td>\
			<td style='text-align: center;'>" + cashObject.Amount.toFixed(2) + "</td>\
		</tr>";

		return cashRow;
	}

	$('#printButton').click(function () {
		$('.print-area').printArea();
	});
});