function getDateFromJSON(jsonDate) {
	return moment(new Date(parseInt(jsonDate.substr(6))))
}

function constructTripRow(tripObject) {
	var newTripRow = "<tr>\
		<td style ='text-align: center;' class='counterCell'></td>\
		<td style ='text-align: center;'>" + getDateFromJSON(tripObject.Date).format('DD/MM/YYYY') + "</td>\
		<td style='text-align: center;'>" + tripObject.VoucherNumber + "</td>\
		<td style='text-align: center;'>" + (tripObject.PONumber ? tripObject.PONumber : "") + "</td>\
		<td style='text-align: center;'>" + tripObject.Site.Customer.Name + "</td>\
		<td style='text-align: center;'>" + tripObject.Site.Name + "</td>\
		<td style='text-align: center;'>" + tripObject.Truck.Number + "</td>\
		<td style='text-align: center;'>" + tripObject.Material.Name + "</td>\
		<td style='text-align: center;'>" + tripObject.QuantityCaption + "</td>\
		<td style='text-align: center;'>" + tripObject.UnitCost.toFixed(2) + "</td>\
		<td style='text-align: center;'>" + tripObject.TripCost.toFixed(2) + "</td>\
		<td style='text-align: center;'>" + tripObject.ExtraCost.toFixed(2) + "</td>\
		<td style='text-align: center;'>" + tripObject.TripTotalCost.toFixed(2) + "</td>\
		<td style='text-align: center;' class='hide-print'>\
			<div style='cursor: pointer;' onclick='editTrip(this);'>\
				<i class='fa fa-edit fa-lg'></i>\
				<i class='fa fa-spinner fa-pulse fa-lg' style='display: none;'></i>\
			</div>\
			<input type='hidden' value='" + tripObject.Id.toString() + "' />\
		</td>";
	if (window.currentWorkObject.addDeleteToTripRow) {
		newTripRow += "<td style='text-align: center;' class='hide-print'>\
				<div style='cursor: pointer;' onclick='deleteTrip(this);'><img src='/Content/images/delete.png' alt='' /></div>\
				<input type='hidden' value='" + tripObject.Id.toString() + "' />\
			</td>";
	}
	newTripRow += "</tr>";
	return newTripRow;
}

function resetTripsTableCounter(tableObject) {
	var cells = $('td.counterCell', tableObject[0]);
	var counter = 1;
	$.each(cells, function (index, cell) {
		$(cell).text(counter++);
	});
}

function initWorkingObject(initObject) {
	var _addDeleteToTripRow = false;
	if (!initObject) {
		if (window.currentWorkObject)
			_addDeleteToTripRow = window.currentWorkObject.addDeleteToTripRow;
	} else {
		_addDeleteToTripRow = initObject.addDeleteToTripRow;
	}

	var workingObject = {
		'ediTD': null,
		'deletedTD': null,
		'currentTrip': null,
		'SelectText': '[SELECT]',
		'addDeleteToTripRow': _addDeleteToTripRow
	};

	window.currentWorkObject = workingObject;
}

function editTrip(divObj) {
	window.currentWorkObject.ediTD = $(divObj).parent();
	var ediIcon = $('i.fa-edit', $(divObj));
	var spinnerIcon = $('i.fa-spinner', $(divObj));

	ediIcon.hide();
	spinnerIcon.show();

	if (!window.lookups) {
		window.lookups = getTripLookups();
	}

	$.ajax({
		type: 'GET',
		dataType: 'json',
		url: '/Trips/GetTrip',
		data: { tripId: $('input', $(divObj).parent()).val() },
		success: function (data) {
			window.currentWorkObject.currentTrip = data;
			initiateAddTripModel(window.lookups);
			ediIcon.show();
			spinnerIcon.hide();
		},
		error: function (jqXHR, textStatus, errorThrown) { alert(errorThrown); }
	});
}

function getTripLookups() {
	return JSON.parse($('#lookupsText').val());
}

function initiateAddTripModel(dataLookup) {
	var selected = '';

	var driversSelect = $('#addTripModal #drivers').empty()
	driversSelect.append($("<option />").val('').text(window.currentWorkObject.SelectText));
	$.each(dataLookup.Drivers, function (index, driver) {
		if (window.currentWorkObject.currentTrip && window.currentWorkObject.currentTrip.Driver.Id == driver.Id)
			selected = 'selected="selected"';
		else
			selected = '';

		driversSelect.append($("<option " + selected + " />").val(driver.Id).text(driver.Name));
	});
	updateTrucksList(driversSelect);

	var customersSelect = $('#addTripModal #customers').empty();
	customersSelect.append($("<option />").val('').text(window.currentWorkObject.SelectText));
	$.each(dataLookup.Customers, function (index, customer) {
		if (window.currentWorkObject.currentTrip && window.currentWorkObject.currentTrip.Site.Customer.Id == customer.Id)
			selected = 'selected="selected"';
		else
			selected = '';
		customersSelect.append($("<option " + selected + " />").val(customer.Id).text(customer.Name));
	});
	updateSitesList(customersSelect);

	var unitsSelect = $('#addTripModal #units').empty();
	unitsSelect.append($("<option />").val('').text(window.currentWorkObject.SelectText));
	$.each(dataLookup.Units, function (index, unit) {
		if (window.currentWorkObject.currentTrip && window.currentWorkObject.currentTrip.Unit.Id == unit.Id)
			selected = 'selected="selected"';
		else
			selected = '';
		unitsSelect.append($("<option " + selected + " />").val(unit.Id).text(unit.Name));
	});
	updateQuantityField(unitsSelect);

	var materialsSelect = $('#addTripModal #materials').empty();
	materialsSelect.append($("<option />").val('').text(window.currentWorkObject.SelectText));
	$.each(dataLookup.Materials, function (index, material) {
		if (window.currentWorkObject.currentTrip && window.currentWorkObject.currentTrip.Material.Id == material.Id)
			selected = 'selected="selected"';
		else
			selected = '';

		materialsSelect.append($("<option " + selected + " />").val(material.Id).text(material.Name));
	});

	if (window.currentWorkObject.currentTrip) {
		$("#addTripModal #purchaseOrder").val(window.currentWorkObject.currentTrip.PONumber);
		$("#addTripModal #voucherNumber").val(window.currentWorkObject.currentTrip.VoucherNumber);
		$("#addTripModal #unitCost").val(window.currentWorkObject.currentTrip.UnitCost.toFixed(2));
		$("#addTripModal #quantity").val(window.currentWorkObject.currentTrip.UnitsQuantity.toFixed(2));
		$("#addTripModal #extraCost").val((window.currentWorkObject.currentTrip.ExtraCost > 0 ? window.currentWorkObject.currentTrip.ExtraCost.toFixed(2) : ''));
		$("#addTripModal #ticketNumber").val(window.currentWorkObject.currentTrip.TicketNumber);
		$('#tripDate').val(getDateFromJSON(window.currentWorkObject.currentTrip.Date).format('DD/MM/YYYY'));

		$('#addTripModal .btn-primary').text('Update Trip');
		$('#addTripModal h4.modal-title').text('Edit Trip: Voucher# ' + window.currentWorkObject.currentTrip.VoucherNumber);

		$('#addTripModal #tripAddedOnDetails').show();

		$('#addTripModal #tripAddedOnDetails #addedOnDiv').text(getDateFromJSON(window.currentWorkObject.currentTrip.AddedOn).format('DD/MM/YYYY HH:mm'));
		$('#addTripModal #tripAddedOnDetails #addedByDiv').text(window.currentWorkObject.currentTrip.AddedBy.Name);

		if(window.currentWorkObject.currentTrip.LastModefiedBy && window.currentWorkObject.currentTrip.LastModifiedOn)
		{
			$('#addTripModal #tripAddedOnDetails #editedOnDiv').text(getDateFromJSON(window.currentWorkObject.currentTrip.LastModifiedOn).format('DD/MM/YYYY HH:mm'));
			$('#addTripModal #tripAddedOnDetails #editByDiv').text(window.currentWorkObject.currentTrip.LastModefiedBy.Name);
		}
		else
		{
			$('#addTripModal #tripAddedOnDetails #editedOnDiv').text('');
			$('#addTripModal #tripAddedOnDetails #editByDiv').text('');
		}
	} else {
		$("#addTripModal #purchaseOrder").val('');
		$("#addTripModal #voucherNumber").val('');
		$("#addTripModal #unitCost").val('');
		$("#addTripModal #quantity").val('');
		$("#addTripModal #extraCost").val('');
		$("#addTripModal #ticketNumber").val('');

		var momentDate = getDateFromJSON(dataLookup.LastInsertionDate.toString());
		$('#tripDate').val(momentDate.format('DD/MM/YYYY'));

		$('#addTripModal .btn-primary').text('Add Trip');
		$('#addTripModal h4.modal-title').text('<Add New Trip>');

		$('#addTripModal #tripAddedOnDetails').hide();
	}

	$('#tripDateFormGroup').datepicker({ autoclose: true, format: 'dd/mm/yyyy' })
		.on('hide.bs.modal', function (event) {
			// prevent datepicker from firing bootstrap modal "show.bs.modal"
			event.stopPropagation();
		});

	$('#addTripModal .btn-primary').removeAttr('disabled');

	//show the modal
	if (window.modalCreated) {
		$('#addTripModal').modal('show');
	} else {
		$('#addTripModal').modal({ backdrop: 'static' }).on('shown.bs.modal',
			function () {
				$('#addTripModal #voucherNumber').focus();
				window.modalCreated = true;
			}
		).on('hide.bs.modal',
			function () {
				initWorkingObject();
			}
		);
	}
}

function updateTrucksList(driversSelectObject) {
	var driverObj = $.grep(window.lookups.Drivers, function (driver, index) {
		return driver.Id == parseInt(driversSelectObject.val());
	});

	if (driverObj.length > 0) {
		var trucksSelect = $('#addTripModal #trucks').empty();
		if (!driverObj[0].IsOutsourced) {
			var truckArray = [];

			truckArray = $.grep(window.lookups.Trucks, function (truck, index) {
				return !truck.IsOutsourced && truck.Driver && truck.Driver.Id == parseInt(driversSelectObject.val());
			});
			
			if (truckArray.length > 0) {
				trucksSelect.append($("<option />").val(truckArray[0].Id).text(truckArray[0].Number));
				trucksSelect.attr('disabled', 'disabled');
			}
		} else {
			var truckArray = $.grep(window.lookups.Trucks, function (truck, index) {
				return truck.IsOutsourced == true;
			});

			var selected = '';
			$.each(truckArray, function (index, truck) {
				if (window.currentWorkObject.currentTrip && window.currentWorkObject.currentTrip.Truck.Id == truck.Id) {
					selected = 'selected="selected"';
				} else {
					selected = '';
				}
				trucksSelect.append($("<option " + selected + " />").val(truck.Id).text(truck.Number));
			});

			trucksSelect.removeAttr('disabled');
		}
	} else {
		$('#addTripModal #trucks').empty();
	}
}

function updateSitesList(customerSelectObject) {
	var customerSites = $.grep(window.lookups.Sites, function (site, index) {
		return site.Customer.Id == parseInt(customerSelectObject.val());
	});

	var sitesSelect = $('#addTripModal #sites').empty();
	sitesSelect.append($("<option />").val('').text(window.currentWorkObject.SelectText));
	$.each(customerSites, function (index, site) {
		if (window.currentWorkObject.currentTrip && window.currentWorkObject.currentTrip.Site.Id == site.Id) {
			selected = 'selected="selected"';
		} else {
			selected = '';
		}
		sitesSelect.append($("<option " + selected + " />").val(site.Id).text(site.Name));
	});
}

function updateQuantityField(unitsselectObject) {
	var selectUnitObj = $.grep(window.lookups.Units, function (unit, index) {
		return unit.Id == parseInt(unitsselectObject.val());
	});

	if (selectUnitObj.length > 0 && selectUnitObj[0].IsOneQuantityOnly == true) {
		$('#addTripModal #quantity').val('1');
		$('#addTripModal #quantity').attr('disabled', 'disabled');
	} else {
		if (selectUnitObj.length > 0) {
			if (!window.currentWorkObject.currentTrip)
				$('#addTripModal #quantity').val('');
			else
				$('#addTripModal #quantity').val(window.currentWorkObject.currentTrip.UnitsQuantity.toFixed(2));

			$('#addTripModal #quantity').removeAttr('disabled');
		} else {
			$('#addTripModal #quantity').removeAttr('disabled');
			$('#addTripModal #quantity').val('');
		}
	}
}

function wireTripEditModalEvents() {
	$('#addTripModal #voucherNumber').change(function () {
		$(this).popover('destroy');
	});

	$('#addTripModal #drivers').change(function () {
		updateTrucksList($(this));
		$(this).popover('destroy');
	});

	$('#addTripModal #customers').change(function () {
		updateSitesList($(this));
		$(this).popover('destroy');
	});

	$('#addTripModal #materials').change(function () {
		$(this).popover('destroy');
	});

	$('#addTripModal #unitCost').change(function () {
		$(this).popover('destroy');
	}); 

	$('#addTripModal #purchaseOrder').change(function () {
		$(this).popover('destroy');
	});

	$('#addTripModal #ticketNumber').change(function () {
		$(this).popover('destroy');
	});

	$('#addTripModal #trucks').change(function () {
		$(this).popover('destroy');
	});

	$('#addTripModal #sites').change(function () {
		$(this).popover('destroy');
	});

	$('#addTripModal #units').change(function () {
		updateQuantityField($(this));
		$(this).popover('destroy');
	});

	$('#addTripModal #quantity').change(function () {
		$(this).popover('destroy');
	});

	$('#addTripModal #extraCost').change(function () {
		$(this).popover('destroy');
	});

	$('#addTripModal #tripDate').change(function () {
		$(this).popover('destroy');
	});

	$('#addTripModal .modal-body').keyup(function (eventObj) {
		if (eventObj.keyCode == 13) {
			if (!$('#addTripModal .btn-primary').attr('disabled'))
				$('#addTripModal .btn-primary').click();
		}
	});

	$('#addTripModal .btn-primary').click(function () {
		var valid = true;

		var numberPattern = /^\d+$/;
		var decimalPattern = /^(?:\d*\.\d{1,2}|\d+)$/;

		var voucherNumber = $('#addTripModal #voucherNumber');
		valid = validateField($('#addTripModal #voucherNumber'), numberPattern, 'Please enter a valid voucher number', '#addTripModal');

		if (valid) {
			var poNumber = $('#addTripModal #purchaseOrder');
			if (poNumber.val())
				valid = validateField(poNumber, numberPattern, 'Please enter a valid P.O number', '#addTripModal');
		}

		if (valid) {
			valid = validateField($('#addTripModal #drivers'), null, 'Please select the driver', '#addTripModal');
		}

		if (valid) {
			valid = validateField($('#addTripModal #customers'), null, 'Please select the customer', '#addTripModal');
		}

		if (valid) {
			valid = validateField($('#addTripModal #sites'), null, 'Please select the site', '#addTripModal');
		}

		if (valid) {
			valid = validateField($('#addTripModal #materials'), null, 'Please select the material', '#addTripModal');
		}

		if (valid) {
			valid = validateField($('#addTripModal #units'), null, 'Please select the unit', '#addTripModal');
		}

		if (valid) {
			valid = validateField($('#addTripModal #quantity'), decimalPattern, 'Please enter a valid quantity value. Example: 120.50', '#addTripModal');
		}

		if (valid) {
			valid = validateField($('#addTripModal #unitCost'), decimalPattern, 'Please enter a valid unit cost. Example: 42.75', '#addTripModal');
		}

		if (valid) {
			var ticketNumber = $('#addTripModal #ticketNumber');
			if (ticketNumber.val())
				valid = validateField(ticketNumber, numberPattern, 'Please enter a valid ticket number', '#addTripModal');
		}

		if (valid) {
			var extraCost = $('#addTripModal #extraCost');
			if (extraCost.val())
				valid = validateField(extraCost, decimalPattern, 'Please enter a valid extar cost. Example: 100.30', '#addTripModal');
		}

		if (valid) {
			$(this).attr('disabled', 'disabled');
			saveNewTrip($(this));
		}
	});

}

function validateField(field, pattern, message, popOverContainer) {
	var result = false;
	if (pattern)
		result = pattern.test(field.val());
	else
		result = field.val();

	if (!result) {
		field.popover({ content: message, trigger: 'focus', container: popOverContainer });
		field.focus();
	} else {
		field.popover('destroy');
	}

	return result;
}

function saveNewTrip(saveButtonObject) {
	var tripObject = {
		'Date': moment($('#addTripModal #tripDate').val(), "DD/MM/YYYY").format('DD/MM/YYYY'),
		'VoucherNumber': parseInt($('#addTripModal #voucherNumber').val()),
		'PONumber': $('#addTripModal #purchaseOrder').val() ? parseInt($('#addTripModal #purchaseOrder').val()) : null,
		'TicketNumber': $('#addTripModal #ticketNumber').val() ? parseInt($('#addTripModal #ticketNumber').val()) : null,
		'UnitCost': parseFloat($('#addTripModal #unitCost').val()),
		'UnitsQuantity': parseFloat($('#addTripModal #quantity').val()),
		'ExtraCost': $('#addTripModal #extraCost').val() ? parseFloat($('#addTripModal #extraCost').val()) : 0,
		'UnitSellingPrice': 0,
		'Material': { 'Id': parseInt($('#addTripModal #materials').val()) },
		'Site': { 'Id': parseInt($('#addTripModal #sites').val()), 'Customer': { 'Id': parseInt($('#addTripModal #customers').val()) } },
		'Driver': { 'Id': parseInt($('#addTripModal #drivers').val()) },
		'Truck': {
			'Id': parseInt($('#addTripModal #trucks').val()),
			'Driver': { 'Id': parseInt($('#addTripModal #drivers').val()) }
		},
		'Unit': { 'Id': parseInt($('#addTripModal #units').val()) }
	};

	if (window.currentWorkObject.currentTrip)
		tripObject.Id = window.currentWorkObject.currentTrip.Id;

	$.ajax({
		type: 'POST',
		dataType: 'json',
		url: '/Trips/SaveTrip',
		data: tripObject,
		success: function (data) {
			saveButtonObject.removeAttr('disabled');
			appendNewTrip(data);
		},
		error: function (jqXHR, textStatus, errorThrown) {
			alert(errorThrown);
			saveButtonObject.removeAttr('disabled');
		}
	});
}

function appendNewTrip(tripObject) {
	var newTripRow = constructTripRow(tripObject);

	if (!window.currentWorkObject.currentTrip) {
		$("#tripsTable").show();
		$("#noResultMessage").hide();
		$("#tripsTable tr:first").after(newTripRow);
		resetAddTripForm();

		showNotification('Trip with voucher#: ' + tripObject.VoucherNumber + ' added successfully <i class="fa fa-check fa-lg"></i>', 'success');
	} else {
		window.currentWorkObject.ediTD.parent().after(newTripRow);
		window.currentWorkObject.ediTD.parent().remove();

		$('#addTripModal').modal('hide');

		showNotification('Trip updated successfully <i class="fa fa-check fa-lg"></i>', 'success ');
	}

	resetTripsTableCounter($("#tripsTable"));
	initWorkingObject();
}

function showNotification(message, type) {
	$.notify({
		message: message
	}, {
		type: type,
		delay: 300,
		z_index: 10000
	});
}

function resetAddTripForm() {
	$("#addTripModal #customers").val($("#addTripModal #customers option:first").val());
	$("#addTripModal #materials").val($("#addTripModal #materials option:first").val());
	$("#addTripModal #units").val($("#addTripModal #units option:first").val());
	$("#addTripModal #sites").val($("#addTripModal #sites option:first").val());

	$("#addTripModal #purchaseOrder").val('');
	$("#addTripModal #voucherNumber").val('');
	$("#addTripModal #unitCost").val('');
	$("#addTripModal #extraCost").val('');
	$("#addTripModal #ticketNumber").val('');
	$("#addTripModal #quantity").val('');

	$('#addTripModal #voucherNumber').focus();
}

function fillDropDown(select, list, textPropertyName) {
	$.each(list, function (index, element) {
		select.append($("<option />").val(element.Id).text(element[textPropertyName]));
	});
}