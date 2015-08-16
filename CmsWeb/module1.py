def BuildForm():
    model.Form = '''
    <form id="wandform" class="form-horizontal">
    <fieldset>

    <!-- Form Name -->
    <legend>FPU Packet Pickup</legend>

    <!-- Paid Options -->
    <div class="form-group">
      <label class="col-md-4 control-label" for="paidoption">Pay Options</label>
      <div class="col-md-4">
      <div class="radio">
        <label for="paidnochange">
          <input type="radio" name="paidoption" id="paidnochange" value="paidnochange" checked="checked">
          No Change
        </label>
        </div>
      <div class="radio">
        <label for="markpaid">
          <input type="radio" name="paidoption" id="markpaid" value="markpaid">
          Mark Paid
        </label>
        </div>
      <div class="radio">
        <label for="marknotpaid">
          <input type="radio" name="paidoption" id="marknotpaid" value="marknotpaid">
          Mark Not Paid
        </label>
        </div>
      </div>
    </div>

    <!-- Picked Up Options -->
    <div class="form-group">
      <label class="col-md-4 control-label" for="pickedup">Pickup Options</label>
      <div class="col-md-4">
      <div class="radio">
        <label for="pickedup">
          <input type="radio" name="pickedup" id="pickedup" value="pickedup" checked="checked">
          Mark Picked Up
        </label>
        </div>
      <div class="radio">
        <label for="notpickedup">
          <input type="radio" name="pickedup" id="notpickedup" value="notpickedup">
          Mark Not Picked Up
        </label>
        </div>
      </div>
    </div>

    <!-- Clear and Action Buttons -->
    <div class="form-group">
      <label class="col-md-4 control-label" for="clear"></label>
      <div class="col-md-8">
        <button id="clear" name="clear" class="btn btn-default">Clear</button>
        <button id="action" name="action" class="btn btn-success">Action</button>
      </div>
    </div>

    <!-- Wand Target -->
    <div class="form-group">
      <label class="col-md-4 control-label" for="wandtarget">Wand Target</label>  
      <div class="col-md-4">
        <input type="hidden" name="pyscript" value="{0}" />
        <input id="wandtarget" name="wandtarget" type="text" placeholder="OrgId-PeopleId" class="form-control input-md" value="{1}">
        <span class="help-block">Keep your cursor blinking in this box before you scan your barcode</span>  
      </div>
    </div>

    <!-- Notice Area -->
    <div class="form-group">
        <div id="output" class="col-md-4 col-md-offset-4"></div>
    </div>

    </fieldset>
    </form>
    '''.format(model.Dictionary("pyscript"), model.Dictionary("wandtarget"))

def BuildJavascript():
    model.Script = '''
    $(function () {
        $("#wandtarget").keypress(function(ev) {
            if (ev.which !== 13)
                return true;
            $.action();
            return false;
        });
        $("#action").click(function(ev) {
            ev.preventDefault();
            $.action();
            return false;
        });
        $.action = function() {
            var v = $("#wandtarget").val();
            if ((v.split("-").length - 1) > 1) {
                $("#wandtarget").val('');
                $("#output").html('');
                return;
            }
            var q = $("#wandform").serialize();
            $.post("/PyScriptForm/", q, function (ret) {
                $("#output").html(ret);
                $("#wandtarget").focus();
            });
        };
        $("#clear").click(function(ev) {
            ev.preventDefault();
            if($("#paidnochange").is(':checked') && $("#pickedup").is(':checked'))
                $("#wandtarget").val('').focus();
            else {
                $('#paidnochange').prop('checked', true);
                $('#pickedup').prop('checked', true);
            }
            $("#output").html("");
        });
        $("#wandtarget").focus();
    });
    '''

def ProcessPost():
    wandtarget = model.Dictionary("wandtarget")
    if '-' not in wandtarget:
        return;

    a = wandtarget.split('-')
    oid = a[0]
    pid = a[1]

    p = model.GetPerson(pid);
    o = model.GetOrganization(oid);

    if p == None:
        print "<h3 class='alert alert-danger'>Person {0} not Found</h3>".format(pid)

    if o == None:
        print "<h3 class='alert alert-danger'>Organization {0} not Found</h3>".format(oid)

    if not model.InOrg(pid, oid):
        print "<h3 class='alert alert-danger'>Person {0} not in {1}</h3>".format(pid, oid)

    if p is not None and o is not None:
        print '''
        <table class="table">
        <tr><td align="right">Name:</td><td><b>{0}</b></td></tr>
        <tr><td align="right">Org:</td><td><b>{1}</b></td></tr>
        </table>
        '''.format(p.Name, o.name)

        if not model.InOrg(pid, oid):
            return

        if model.Dictionary("paidoption") == "markpaid":
            model.AddSubGroup(pid, oid, "pay online")
            print "<h3 class='alert alert-warning'>Marked Now As Paid</h3>"
        elif model.Dictionary("paidoption") == "marknotpaid":
            model.RemoveSubGroup(pid, oid, "pay online")
            print "<h3 class='alert alert-warning'>Marked Now As Not Paid</h3>"
        else:
            if model.InSubGroup(pid, oid, "pay online"):
                print "<h3 class='alert alert-success'>Packet Paid For Already</h3>"
            else:
                print "<h3 class='alert alert-danger'>Packet Not Paid For</h3>"

        if model.Dictionary("pickedup") == "notpickedup":
            model.RemoveSubGroup(pid, oid, "packet-picked-up")
            print "<h3 class='alert alert-warning'>Packet Now Marked As Not Picked Up</h3>"
        else:
            if model.InSubGroup(pid, oid, "packet-picked-up"):
                print "<h3 class='alert alert-danger'>Packet Already picked up</h3>"
            else:
                model.AddSubGroup(pid, oid, "packet-picked-up")
                print "<h3 class='alert alert-success'>Packet Now Marked as Picked Up</h3>"

if model.HttpMethod == 'get':
    BuildForm()
    BuildJavascript()

if model.HttpMethod == 'post':
    ProcessPost()