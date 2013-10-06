$(document).ready(function() {

	// Construction de la table
	var oTable = $('#tableau').dataTable( {
		"oLanguage": {"sUrl": "assets/datatables.french.lang"},
		"iDisplayLength": 25,
		"aoColumns": [
			{ "sTitle": "id", "bVisible": false },
			{ "sTitle": "Date", "sType": "date", "sClass": "center" },
			{ "sTitle": "Label", "sWidth": "50%" },
			{ "sTitle": "Montant", "sClass": "center" },
			{ "sTitle": "Catégorie", "sWidth": "20%" },
			{ "sTitle": "Commentaire", "sWidth": "30%" }
		],
		"aaSorting": [[ 1, "desc" ]], // Tri sur la colonne date
		"bProcessing": true, // Affichage d'une pop-up lors du chargement des données
		"bServerSide": true,
		"sAjaxSource": "../getActions",
		"fnServerData": function ( sSource, aoData, fnCallback, oSettings ) {
			var filtres = [{ nom:"AllOpen", dbName:"Perso" }, { recherche:"test", dbName:"Perso" }];
			oSettings.jqXHR = $.ajax({
				dataType: 'json',
				type: "POST",
				url: sSource,
				data: JSON.stringify({DTparams: aoData, filtres: filtres}),
				processData: false,
				contentType: "application/json; charset=UTF-8",
				success: fnCallback
			});
		},
		"fnInitComplete": function(oSettings, json) { // Mise en form Bootstrap de certains composants
			$('div.dataTables_filter input')
				.attr('placeholder', 'Rechercher')
				.addClass('form-control input-sm')
				.css('width', '250px');
			$('select[name=transactions_length]')
				.addClass('form-control input-sm')
				.css('width', '75px');
			$('div.dataTables_info').css('margin-bottom', '30px');
		}
	} );	
	
} );;