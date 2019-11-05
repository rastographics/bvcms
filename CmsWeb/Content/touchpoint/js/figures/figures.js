new Vue({
    el: '#fig',
    data: {
        Programs: null,
        Divisions: null,
        Organizations: null,
        ProgramId: -1,
        DivisionId: -1,
        OrganizationId: -1
    },
    methods: {
        myFunctionOnLoad: function () {
            this.GetPrograms();
            console.log(this.Test);
        },
        GetPrograms: function () {
            this.$http.get('/Figures/GetPrograms').then(
                response => {
                    if (response.status === 200) {
                        this.Programs = response.body;
                        console.log(this.Programs);
                    }
                    else {
                        console.log(response);
                        warning_swal('Warning!', 'Something went wrong, try again later');
                    }
                },
                err => {
                    console.log(err);
                    error_swal('Fatal Error!', 'We are working to fix it');
                }
            );
        },
        OnChangeProgram: function () {
            this.DivisionsId = -1;
            this.Organizations = null;
            this.Divisions = null;
            if (this.ProgramId > 0) {
                console.log(this.ProgramId);
                this.Divisions = this.Programs.find(x => x.Id === this.ProgramId).DivList;
            }
        },
        OnChangeDivision: function () {
            this.OrganizationId = -1;
            this.Organizations = null;
            if (this.DivisionId > 0) {
                this.Organizations = this.Divisions.find(x => x.Id === this.DivisionId).OrgList;
            }
        }
    },
    created: function () {
        this.myFunctionOnLoad();
    }
});
