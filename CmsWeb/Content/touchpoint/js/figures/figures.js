new Vue({
    el: '#fig',
    data: {
        Programs: [],
        Divisions: [],
        Organizations: [],
        SelectedOrganizations: [],
        CurrentOrganization: [],
        ProgramId: -1,
        DivisionId: -1,
        OrganizationId: -1,
        SelectedOrganizationsId: -1
    },
    methods: {
        myFunctionOnLoad: function () {
            this.GetPrograms();
        },
        GetPrograms: function () {
            this.$http.get('/Figures/GetPrograms').then(
                response => {
                    if (response.status === 200) {
                        this.Programs = response.body;
                    }
                    else {
                        warning_swal('Warning!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    error_swal('Fatal Error!', 'We are working to fix it');
                }
            );
        },
        OnChangeProgram: function () {
            this.DivisionsId = -1;
            if (this.SelectedOrganizations.length > 0) {
                this.SelectedOrganizations.forEach(this.RemoveAllSelectedOrganizations);
            }
            this.Organizations = [];
            this.SelectedOrganizations = [];
            this.Divisions = null;
            if (this.ProgramId > 0) {
                this.Divisions = this.Programs.find(x => x.Id === this.ProgramId).DivList;
            }
        },
        OnChangeDivision: function () {
            this.OrganizationId = -1;
            if (this.SelectedOrganizations.length > 0) {
                this.SelectedOrganizations.forEach(this.RemoveAllSelectedOrganizations);
            }
            this.Organizations = [];
            this.SelectedOrganizations = [];
            if (this.DivisionId > 0) {
                this.Organizations = this.Divisions.find(x => x.Id === this.DivisionId).OrgList;
            }
        },
        AddOrganization: function () {
            var selected = this.CurrentOrganization.slice(0);
            for (var i = 0; i < selected.length; ++i) {
                var index = this.Organizations.findIndex(function (el) {
                    return el.Id === selected[i];
                });
                var item = this.Organizations[index];

                this.Organizations.splice(index, 1);
                this.SelectedOrganizations.push(item);
            }
        },
        RemoveOrganization: function () {
            var selected = this.CurrentOrganization.slice(0);
            for (var i = 0; i < selected.length; ++i) {
                var index = this.SelectedOrganizations.findIndex(function (el) {
                    return el.Id === selected[i];
                });
                var item = this.SelectedOrganizations[index];

                this.SelectedOrganizations.splice(index, 1);
                this.Organizations.push(item);
            }
        },
        RemoveAllSelectedOrganizations: function () {
            for (let j = 0; this.SelectedOrganizations.length; j++) {
                var selected = this.SelectedOrganizations.slice(0);
                for (var i = 0; i < selected.length; ++i) {
                    var index = this.SelectedOrganizations.findIndex(function (el) {
                        return el.Id === selected[i].Id;
                    });
                    var item = this.SelectedOrganizations[index];

                    this.SelectedOrganizations.splice(index, 1);
                    this.Organizations.push(item);
                }
            }
        }
    },
    created: function () {
        this.myFunctionOnLoad();
    }
});
