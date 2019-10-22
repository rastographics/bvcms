new Vue({
    el: '#fig',
    data: {
        Programs: [{}],
        ProgramId: -1,
        Test: 'Prueba'
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
        }
    },
    created: function () {
        this.myFunctionOnLoad();
    }
});

//$(function () {
//    google.load("visualization", "1", { packages: ["corechart"] });

//    $('#DrawChart').click(function () {
//        var e = document.getElementById('Attendance_chart_display');
//        e.style.display = 'block';
//        var selectedValues = $('#Organization-DropdownID').val();
//        $("#Attendance_chart_display").load('/Figures/Figures/AttendanceChartDisplayView', { orgIdsArr: selectedValues });
//    });
//});
