$(document).ready(function() {

	// Construction de la table
	var oTable = $('#tableau').dataTable( {
		"oLanguage": {"sUrl": "assets/datatables.french.lang"},
		"iDisplayLength": 25,
		"aoColumns": [
			{ "sTitle": "id" },
			{ "sTitle": "Contexte" },
			{ "sTitle": "Sujet" },
			{ "sTitle": "Contenu", "sWidth": "20%",
			  "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
				nTd.innerHTML = sData.replace(/\r\n|\r|\n/g, '<br />');
			  }
			},
			{ "sTitle": "Liens" },
			{ "sTitle": "Deadline" },
			{ "sTitle": "Destinataire" },
			{ "sTitle": "Statut" },
			{ "sTitle": "DB" },
			{ "sTitle": "Ref" }
		],
		"aaSorting": [[ 1, "desc" ]], // Tri sur la colonne date
		"bProcessing": true, // Affichage d'une pop-up lors du chargement des données
		"bServerSide": true,
		"sAjaxSource": "../getActions",
		"fnServerData": function ( sSource, aoData, fnCallback, oSettings ) {
			var filtres = [{ recherche:"test", dbName:"Perso" }];
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