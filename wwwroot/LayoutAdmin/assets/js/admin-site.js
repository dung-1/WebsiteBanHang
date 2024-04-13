/*\
|*|
|*|  :: cookies.js ::
|*|
|*|  A complete cookies reader/writer framework with full unicode support.
|*|
|*|  Revision #1 - September 4, 2014
|*|
|*|  https://developer.mozilla.org/en-US/docs/Web/API/document.cookie
|*|  https://developer.mozilla.org/User:fusionchess
|*|
|*|  This framework is released under the GNU Public License, version 3 or later.
|*|  http://www.gnu.org/licenses/gpl-3.0-standalone.html
|*|
|*|  Syntaxes:
|*|
|*|  * docCookies.setItem(name, value[, end[, path[, domain[, secure]]]])
|*|  * docCookies.getItem(name)
|*|  * docCookies.removeItem(name[, path[, domain]])
|*|  * docCookies.hasItem(name)
|*|  * docCookies.keys()
|*|
\*/
// Write your JavaScript code.
function InitDataTable(idTable) {
    $(idTable).DataTable({
        //"lengthMenu": [[50, 75, 150, -1], [50, 75, 150, "All"]],
        "responsive": true, "lengthChange": false, "autoWidth": false,
        "buttons": ["copy", "csv", "excel", "pdf", "print"],
        "language": {
            "sProcessing": "Đang xử lý...",
            "sLengthMenu": "Xem _MENU_ mục",
            "sZeroRecords": "Không tìm thấy dòng nào phù hợp",
            "sInfo": "Đang xem từ _START_ đến _END_ trong tổng số _TOTAL_ mục",
            "sInfoEmpty": "Đang xem 0 đến 0 trong tổng số 0 mục",
            "sInfoFiltered": "(được lọc từ _MAX_ mục)",
            "sInfoPostFix": "",
            "sSearch": "Tìm kiếm:",
            "sUrl": "",
            "oPaginate": {
                "sFirst": "Đầu",
                "sPrevious": "Trước",
                "sNext": "Tiếp",
                "sLast": "Cuối"
            }
        },
        //paging: true,
        //serverSide: true,
        //listAction: {
        //    ajaxFunction: "",
        //    inputFilter: function () {
        //        return "";
        //    }
        //},

        "columnDefs": [
            //{ "orderable": false, "targets": [0] },
            { "targets": 0, "visible": false },
        ],
        //"ordering": false
        "aaSorting": [] //disable sort default
        //https://www.codegrepper.com/code-examples/javascript/disable+default+sorting+in+datatable
    });
    //.buttons().container().appendTo(idTable + '_wrapper .col-md-6:eq(0)');
};

var docCookies = {
    getItem: function (sKey) {
        if (!sKey) { return null; }
        return decodeURIComponent(document.cookie.replace(new RegExp("(?:(?:^|.*;)\\s*" + encodeURIComponent(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=\\s*([^;]*).*$)|^.*$"), "$1")) || null;
    },
    setItem: function (sKey, sValue, vEnd, sPath, sDomain, bSecure) {
        if (!sKey || /^(?:expires|max\-age|path|domain|secure)$/i.test(sKey)) { return false; }
        var sExpires = "";
        if (vEnd) {
            switch (vEnd.constructor) {
                case Number:
                    sExpires = vEnd === Infinity ? "; expires=Fri, 31 Dec 9999 23:59:59 GMT" : "; max-age=" + vEnd;
                    break;
                case String:
                    sExpires = "; expires=" + vEnd;
                    break;
                case Date:
                    sExpires = "; expires=" + vEnd.toUTCString();
                    break;
            }
        }
        document.cookie = encodeURIComponent(sKey) + "=" + encodeURIComponent(sValue) + sExpires + (sDomain ? "; domain=" + sDomain : "") + (sPath ? "; path=" + sPath : "") + (bSecure ? "; secure" : "");
        return true;
    },
    removeItem: function (sKey, sPath, sDomain) {
        if (!this.hasItem(sKey)) { return false; }
        document.cookie = encodeURIComponent(sKey) + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT" + (sDomain ? "; domain=" + sDomain : "") + (sPath ? "; path=" + sPath : "");
        return true;
    },
    hasItem: function (sKey) {
        if (!sKey) { return false; }
        return (new RegExp("(?:^|;\\s*)" + encodeURIComponent(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=")).test(document.cookie);
    },
    keys: function () {
        var aKeys = document.cookie.replace(/((?:^|\s*;)[^\=]+)(?=;|$)|^\s*|\s*(?:\=[^;]*)?(?:\1|$)/g, "").split(/\s*(?:\=[^;]*)?;\s*/);
        for (var nLen = aKeys.length, nIdx = 0; nIdx < nLen; nIdx++) { aKeys[nIdx] = decodeURIComponent(aKeys[nIdx]); }
        return aKeys;
    }
};


/* collapse/expand */
if (docCookies.hasItem("sidebarstate")) {
    $("body").addClass(docCookies.getItem("sidebarstate"));
}

/*global datatable defaults*/
//$.extend(true, $.fn.dataTable.defaults, {

//});

/* automatically defaults all submit button to be disabled */
function useSubmitClass() {
    $('form').submit(function () {
        if ($(this).valid()) {
            $(this).find(':submit').attr('disabled', 'disabled');
        }
    });
}

function useDeleteConfirmation() {
    $('.btn-delete').on("click", function (e) {
        e.preventDefault();

        var choice = confirm("Are you sure you want to delete?");

        if (choice) {
            window.location.href = $(this).attr('href');
        }
    });
}

function InitDialogElFinder(idText, idButton) {
    $(idButton).click(function (e) {
        $('#editor').remove();

        var myCommands = elFinder.prototype._options.commands;

        // Not yet implemented commands in elFinder.NetCore
        var disabled = ['callback', 'chmod', 'editor', 'netmount', 'ping', 'search', 'zipdl', 'help'];
        elFinder.prototype.i18.en.messages.TextArea = "Edit";

        $.each(disabled, function (i, cmd) {
            (idx = $.inArray(cmd, myCommands)) !== -1 && myCommands.splice(idx, 1);
        });

        var options = {
            baseUrl: "/lib/elfinder/",
            url: "/file-system/connector",
            rememberLastDir: false,
            commands: myCommands,
            lang: 'vi',
            uiOptions: {
                toolbar: [
                    ['back', 'forward'],
                    ['reload'],
                    ['home', 'up'],
                    ['mkdir', 'mkfile', 'upload'],
                    ['open', 'download'],
                    ['undo', 'redo'],
                    ['info'],
                    ['quicklook'],
                    ['copy', 'cut', 'paste'],
                    ['rm'],
                    ['duplicate', 'rename', 'edit'],
                    ['selectall', 'selectnone', 'selectinvert'],
                    ['view', 'sort']
                ]
            },
            lang: 'vi',
            getFileCallback: function (files, fm) {
                //$(idText).val(files.path);
                $('#editor').remove();

                $(idText).val('/' + files[0].path);
                $('#PictureView').attr('src', '/' + files[0].path);
            },
            commandsOptions: {
                getfile: {
                    multiple: true,
                    oncomplete: 'close',
                    folders: false
                }
            },
            width: 1024,
            destroyOnClose: true
        };

        $('<div id="editor" />').dialogelfinder(options).elfinder('instance')
    });
};

