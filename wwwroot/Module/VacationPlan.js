﻿$(document).ready(() => {

    GetVacationType();
});

FindEmployee = () => {
    if ($('#employeeName').val() == '') {
        $('#ddlEmployees').html(`<option value="">**Not Found**</option>`);
    }
    else {
        $.ajax(
            {
                url: `/api/VacationPlansApi/${$('#employeeName').val()}`,
                method: 'GET',
                cache: false,
                success: (data) => {
                    let Employee = '';
                    Employee += `<option value="">**Items Found(${data.length})**</option>`;
                    for (x in data) {
                        Employee += `<option value="${data[x].id}">${data[x].id}-${data[x].name}</option>`;
                    }
                    $('#ddlEmployees').html(Employee);
                }
                
            });
    }
}

GetVacationType = () => {
    $.ajax({
        url: '/VacationPlans/GetVacationTypes',
        method: 'GET',
        cache: false,
        success: (result) => {
            let Vacations = '';
            Vacations += `<option value="">**Select Vacation(${result.length})**</option>`;
            for (x in result) {
                Vacations += `<option value="${result[x].id}"; style="color:#ffff; background-color:${result[x].backGroundColor};">${result[x].id}-${result[x].vacationName}========> Days:- (${result[x].numberDays})</option>`;
            }
            $('#ddlVacationType').html(Vacations);
        }
    });
}