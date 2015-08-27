<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Admin.aspx.cs" Inherits="Admin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <asp:PlaceHolder runat="server">
        <script src="<%: ResolveUrl("~/Scripts/modernizr-2.5.3.js") %>"></script>
    </asp:PlaceHolder>
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
    <script src="http://code.jquery.com/jquery-1.9.1.js"></script>
    <script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js"></script>
    <link href="Content/bootstrap.cosmo.min.css" rel="stylesheet" />

    <style>
        .ui-autocomplete {
            font-size: 11px;
            text-align: left;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            $('#<%=txtQueueList.ClientID%>').autocomplete({
                minLength: 2,
                source: function (request, response) {
                    $.ajax({
                        url: "Admin.aspx/GetAgentList",
                        data: "{ 'pre':'" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            response($.map(data.d, function (item) {
                                return { value: item }
                            }))
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            alert(textStatus);
                        }
                    });
                }
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="shadowBox">
            <div class="page-container">
                <div class="container">
                    <div class="jumbotron">
                        <p class="text-danger">Agent Spare Time Optimization Watcher </p>
                        <span class="text-info">Agent Call Status per Queue</span>
                    </div>
                    <div>
                        <br />
                        <span class="text-info">Select a Queue Name</span>
                        <asp:TextBox ID="txtQueueList" runat="server" Width="350px" CssClass="textboxAuto" Font-Size="13px" OnTextChanged="txtQueueList_TextChanged"></asp:TextBox>
                        <br /><br />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12 ">
                    <div class="table-responsive">
                        <asp:GridView ID="grdAgentInfo" runat="server" Width="100%" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="Agent" EmptyDataText="There are no data records to display.">
                            <Columns>
                                <asp:BoundField DataField="Agent" HeaderText="Agent Name" ReadOnly="True" SortExpression="AgentName" />
                                <asp:BoundField DataField="Status" HeaderText="Status Of the Agent" SortExpression="Status" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>


    </form>
</body>
</html>
