//function agregarContactoAhtml(datos) {
//    $('#tblDatosContacto').remove();
//    if (datos.contactos.length > 0) {
//        $('#div_tblDatosContacto').append(
//            ' <table id="tblDatosContacto" class="table table-hover" align="center"> '
//            + '<thead> '
//            + ' <tr class="d-flex">'
//            + ' <th scope="col" class="col-2">' + ' Tipo' + ' </th>'
//            + ' <th scope="col" class="col-4">' + ' Valor' + ' </th>'
//            + ' <th scope="col" class="col-5">' + ' Observacion' + ' </th>'
//            + ' <th scope="col" class="col-1">' + ' ' + ' </th>'
//            + ' </thead>'
//            + ' </table> ');

//        let listaContacto = "";
//        for (var i = 0; i < datos.contactos.length; i++) {
//            listaContacto = listaContacto + '<tr class="d-flex"> ' +
//                '<td class="col-2">' + datos.contactos[i].ContactoTipo + '</td> ' +
//                '<td class="col-4">' + datos.contactos[i].valor + '</td>' +
//                '<td class="col-5">' + datos.contactos[i].observaciones + '</td>' +
//                '<td class="col-1">' + '<i style="cursor:pointer;" class="fa fa-trash text-danger" id="btn_quitar_' + datos.contactos[i].idContacto + '"' + '></i> ' + '</td>' +
//                '</tr>';


//        }

//        $('#tblDatosContacto').append('<tbody>' + listaContacto + '</tbody>');

//        for (var i = 0; i < datos.contactos.length; i++) {
//            let _idContacto = datos.contactos[i].idContacto
//            document.getElementById("btn_quitar_" + datos.contactos[i].idContacto).addEventListener("click", function () { quitarContacto(_idContacto); }, false);
//        }
//    }
//}
