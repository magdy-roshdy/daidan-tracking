var phonecatApp = angular.module('adminPercentageApp', []);

phonecatApp.controller('AdminPercentageController', function ($scope, $http) {
	$http.get('/AdminPercentage/GetMonthMaterials', {
		params: { 'monthId': $('#monthId').val() }
	}).success(function (data) {
		$scope.materialsPercentages = data.MaterialPercentages;
		$scope.materislList = data.MaterialsList;
		$scope.customersList = data.CustomersList;
	}
	);

	$scope.monthCaption = $('#monthCaption').val();
	$scope.deletedMaterials = [];
	$scope.deletedCustomers = [];

	$scope.materialClick = function (materialObject) {
		$scope.currentMaterial = materialObject;
	};

	$scope.saveCustomer = function (materialObject) {
		var newCustomer = {
			'Id': 0,
			'Customer':
				{
					'Name': $('#addCustomerModal #customer').children(':selected').text(),
					'Id': parseInt($('#addCustomerModal #customer').val())
				},
			'Amount': parseFloat($('#addCustomerModal #amount').val()),
			'IsFixedAmount': $("#addCustomerModal #isFixed").is(':checked')
		};
		$scope.currentMaterial.CustomersPercentage.push(newCustomer);

		$('#addCustomerModal #closeButton').click();
	};

	$scope.openAddCustomerModal = function () {
		$("#addCustomerModal #isFixed")[0].checked = false;
		$('#addCustomerModal #amount').val('');

		$('#addCustomerModal').modal().show();
	};

	$scope.getMaterialRemainingCustomers = function () {
		var customers = [];
		if ($scope.currentMaterial) {
			$.each($scope.customersList, function (index, customer) {
				var _customer = $.grep($scope.currentMaterial.CustomersPercentage, function (customerPercentage, i) {
					return (customerPercentage.Customer.Id == customer.Id);
				});

				if (_customer.length == 0) {
					customers.push(customer);
				}
			});
		}

		return customers;
	};

	$scope.deletedCustomer = function (customerPercentage) {
		if (confirm('Are you sure?')) {
			var index = -1;
			$.each($scope.currentMaterial.CustomersPercentage, function (i, _customer) {
				if (_customer.Customer.Id == customerPercentage.Customer.Id) {
					index = i;
				}
			});

			if (index > -1) {
				$scope.currentMaterial.CustomersPercentage.splice(index, 1);

				if (customerPercentage.Id > 0)
					$scope.deletedCustomers.push(customerPercentage.Id);
			}
		}
	};

	$scope.openAddMaterialModal = function () {
		$("#addMaterialModal #isFixed")[0].checked = false;
		$('#addMaterialModal #amount').val('');

		$('#addMaterialModal').modal().show();
	};

	$scope.getRemainingMaterials = function () {
		var materials = [];
		$.each($scope.materislList, function (index, material) {
			var _material = $.grep($scope.materialsPercentages, function (materialPercentage, i) {
				return (materialPercentage.Material.Id == material.Id);
			});

			if (_material.length == 0) {
				materials.push(material);
			}
		});

		return materials;
	};

	$scope.saveMaterial = function () {
		var newMaterialPercentage = {
			'Id': 0,
			'Month':
				{
					'Id': parseInt($('#monthId').val())
				},
			'Material':
				{
					'Name': $('#addMaterialModal #material').children(':selected').text(),
					'Id': parseInt($('#addMaterialModal #material').val())
				},
			'Amount': parseFloat($('#addMaterialModal #amount').val()),
			'IsFixedAmount': $("#addMaterialModal #isFixed").is(':checked'),
			'CustomersPercentage': []
		};
		$scope.materialsPercentages.push(newMaterialPercentage);

		$('#addMaterialModal #closeButton').click();
	};

	$scope.deleteMaterial = function (materialPercentage) {
		if (confirm('Are you sure?')) {
			var index = -1;
			$.each($scope.materialsPercentages, function (i, _material) {
				if (_material.Material.Id == materialPercentage.Material.Id) {
					index = i;
				}
			});

			if (index > -1) {
				$scope.materialsPercentages.splice(index, 1);

				if (materialPercentage.Id > 0) {
					$scope.deletedMaterials.push(materialPercentage.Id);

					if ($scope.currentMaterial.Material.Id == materialPercentage.Material.Id)
						$scope.currentMaterial = null;
				}
			}
		}
	};

	$scope.saveChanges = function () {
		$('#saveChangesButton').attr('disabled', 'disabled');

		$http.post('/AdminPercentage/SaveMonthPercentages',
				{
					'monthMaterialPercentages': $scope.materialsPercentages, 'monthId': $('#monthId').val(),
					'materialsToDelete': $scope.deletedMaterials, 'customersToDelete': $scope.deletedCustomers
				}
			).
		  then(function (response) {
		  	window.location = response.data.url;
		  }, function (response) {
		  	alert('Some error happened while saving month percentage information, please contact the system engineer.');
		  	$('#saveChangesButton').removeAttr('disabled');
		  });
	};

	$scope.getSelectedCalss = function (material) {
		if ($scope.currentMaterial && $scope.currentMaterial.Material.Id == material.Material.Id) {
			return "panel-primary";
		}
		else {
			return "panel-default";
		}
	};
});