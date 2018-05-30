//Document load
$(document).ready(function () {
    if ($("#CurrentPage").val() == "StudentAgreement") {
        Erasmus.Student.BindPageControls();
    }

    if ($("#CurrentPage").val() == "UniversityAgreement") {
        Erasmus.UniversityAgreement.Initialize();
    }
});

//PageContext
(function (Erasmus, $, undefined) {
    var context = {
        Selects : []
    };
    Erasmus.PageContext = context;
})(window.Erasmus = window.Erasmus || {}, jQuery);

//Settings
(function (Erasmus, $, undefined) {
    var settings = {
        StudentAgreement : {
            SourceUniChangeUrl: "/Student/GetSourceUniData",
            FacultyDataUrl: "/Student/GetFacultyDataByUniId",
            FosDataUrl: "/Student/GetFosDataByFacultyId",
            StudySubjectsUrl: "/Student/GetStudySubjects",
            SemesterDataUrl: "/Student/GetSemesterData"
        },
        UniversityAgreement : {
            TargetUniversityUrl : "/Admin/GetTargetAgreementUniversities"
        }
    };

    //Registers component to global scope
    Erasmus.Settings = settings;
})(window.Erasmus = window.Erasmus || {}, jQuery);

//Student agreement
(function (Erasmus, $, undefined) {
    var student = {
        BindPageControls: function () {
            if (isEdit != "True") {
                disableDefaultSelectInputs();
            }
            $("#srcUni").on("select2:select", function (selected) {
                $.getJSON(Erasmus.Settings.StudentAgreement.SourceUniChangeUrl + "/?universityId=" + selected.params.data.id, function (data) {
                    $("#trgUni").val(null).trigger("change");
                    $('#trgUni').empty();

                    $("#srcFaculty").val(null).trigger("change");
                    $('#srcFaculty').empty();

                    var trgUniDropdownData = [];
                    $.each(data.TargetUniversities, function (key, val) {
                        trgUniDropdownData.push({ id: val.Key, text: val.Value });
                    });

                    var defaultTrgUniOption = new Option("Select target university", null, true, true);
                    $("#trgUni").append(defaultTrgUniOption).trigger('change');
                    $("#trgUni").select2({ data: trgUniDropdownData });

                    var srcFacultyDropdownData = [];
                    $.each(data.SourceFaculties, function (key, val) {
                        srcFacultyDropdownData.push({ id: val.Key, text: val.Value });
                    });

                    var defaultSrcFacultyOption = new Option("Select source faculty", null, true, true);
                    $("#srcFaculty").append(defaultSrcFacultyOption).trigger('change');
                    $("#srcFaculty").select2({ data: srcFacultyDropdownData });

                    if (trgUniDropdownData.length == 0) {
                        disableDefaultSelectInputs();
                    } else {
                        enableSelectInput("#trgUni");
                        enableSelectInput("#srcFaculty");
                    }
                });
            });

            $("#trgUni").on("select2:select", function(selected) {
                $.getJSON(Erasmus.Settings.StudentAgreement.FacultyDataUrl + "/?universityId=" + selected.params.data.id, function (data) {
                    $("#trgFaculty").val(null).trigger("change");
                    $('#trgFaculty').empty();
                    var trgFacultyDropdownData = [];
                    $.each(data, function (key, val) {
                        trgFacultyDropdownData.push({ id: val.Key, text: val.Value });
                    });

                    var defaultOption = new Option("Select target faculty", null, true, true);
                    $("#trgFaculty").append(defaultOption).trigger('change');
                    $("#trgFaculty").select2({ data: trgFacultyDropdownData });

                    if (trgFacultyDropdownData.length == 0) {
                        disableSelectInput("#trgFaculty");
                    } else {
                        enableSelectInput("#trgFaculty");
                    }
                });
            });

            $("#srcFaculty").on("select2:select", function (selected) {
                $.getJSON(Erasmus.Settings.StudentAgreement.FosDataUrl + "/?facultyId=" + selected.params.data.id, function (data) {
                    $("#srcFos").val(null).trigger("change");
                    $('#srcFos').empty();
                    var dropdownData = [];
                    $.each(data, function (key, val) {
                        dropdownData.push({ id: val.Key, text: val.Value });
                    });

                    var defaultOption = new Option("Select source field of study", null, true, true);
                    $("#srcFos").append(defaultOption).trigger('change');
                    $("#srcFos").select2({ data: dropdownData });

                    if (dropdownData.length == 0) {
                        disableSelectInput("#srcFos");
                    } else {
                        enableSelectInput("#srcFos");
                    }
                });
            });

            $("#trgFaculty").on("select2:select", function (selected) {
                $.getJSON(Erasmus.Settings.StudentAgreement.FosDataUrl + "/?facultyId=" + selected.params.data.id, function (data) {
                    $("#trgFos").val(null).trigger("change");
                    $('#trgFos').empty();
                    var dropdownData = [];
                    $.each(data, function (key, val) {
                        dropdownData.push({ id: val.Key, text: val.Value });
                    });

                    var defaultOption = new Option("Select target field of study", null, true, true);
                    $("#trgFos").append(defaultOption).trigger('change');
                    $("#trgFos").select2({ data: dropdownData });

                    if (dropdownData.length == 0) {
                        disableSelectInput("#trgFos");
                    } else {
                        enableSelectInput("#trgFos");
                    }
                });
            });

            $("#srcFos").on("select2:select", function (selected) {
                checkForBothFos();
            });

            $("#trgFos").on("select2:select", function (selected) {
                checkForBothFos();
            });

            function checkForBothFos() {
                if ($("#srcFos").val() != "null" && $("#trgFos").val() != "null") {
                    $.getJSON(Erasmus.Settings.StudentAgreement.SemesterDataUrl + "/?sourceFosId=" + $("#srcFos").val() + "&targetFosId="+ $("#trgFos").val(), function (data) {
                        $("#semester").val(null).trigger("change");
                        $('#semester').empty();
                        var dropdownData = [];
                        $.each(data, function (key, val) {
                            dropdownData.push({ id: val.Key, text: val.Value });
                        });

                        var defaultOption = new Option("Select semester", null, true, true);
                        $("#semester").append(defaultOption).trigger('change');
                        $("#semester").select2({ data: dropdownData });

                        var defaultDisabledOption = new Option("Select semester", null, true, true);
                        $("#disabledSemester").append(defaultDisabledOption).trigger('change');
                        $("#disabledSemester").select2({ data: dropdownData });

                        if (dropdownData.length == 0) {
                            disableSelectInput("#semester");
                        } else {
                            enableSelectInput("#semester");
                        }
                    });
                }
            }

            $("#semester").on("select2:select", function (selected) {
                if ($("#srcFos").val() != null && $("#trgFos").val() != null) {
                    $.getJSON(Erasmus.Settings.StudentAgreement.StudySubjectsUrl + "/?sourceFosId=" + $("#srcFos").val() + "&targetFosId=" + $("#trgFos").val()+"&semester="+ selected.params.data.id,
                        function (data) {
                            srcCredits = 0;
                            trgCredits = 0;
                            $("#srcTotal").html("Total: " + srcCredits);
                            $("#trgTotal").html("Total: " + trgCredits);
                            $("#srcStudySubjects").val(null).trigger("change");
                            $('#srcStudySubjects').empty();

                            $("#trgStudySubjects").val(null).trigger("change");
                            $('#trgStudySubjects').empty();

                            var srcDropdownData = [];
                            srcStudySubjectIndex = [];
                            $.each(data.SourceStudySubjects,
                                function(key, val) {
                                    srcDropdownData.push({ id: val.Key, text: val.Value + " (" + val.Credits + ")" });
                                    srcStudySubjectIndex.push({ Id: val.Key, Credits: val.Credits });
                                });

                            var trgDropdownData = [];
                            trgStudySubjectIndex = [];
                            $.each(data.TargetStudySubjects,
                                function (key, val) {
                                    trgDropdownData.push({ id: val.Key, text: val.Value + " (" + val.Credits + ")" });
                                    trgStudySubjectIndex.push({ Id: val.Key, Credits: val.Credits });
                                });

                            $("#srcStudySubjects").select2({ data: srcDropdownData, allowClear: true });
                            $("#trgStudySubjects").select2({ data: trgDropdownData, allowClear: true });

                            $("#disabledSemester").val("" + selected.params.data.id);
                            $("#disabledSemester").trigger('change');

                            if (srcDropdownData.length == 0) {
                                disableSelectInput("#srcStudySubjects");
                            } else {
                                enableSelectInput("#srcStudySubjects");
                            }

                            if (trgDropdownData.length == 0) {
                                disableSelectInput("#trgStudySubjects");
                            } else {
                                enableSelectInput("#trgStudySubjects");
                            }
                        });
                } else {
                    //TODO highlight input why we can't do a query...
                }

            });

            $("#srcStudySubjects").on("select2:select", function (selected) {
                var credits = parseInt($.grep(srcStudySubjectIndex, function (obj) { return obj.Id == selected.params.data.id; })[0].Credits);
                srcCredits += credits;
                $("#srcTotal").html("Total: " + srcCredits)
            });

            $("#srcStudySubjects").on("select2:unselect", function(selected) {
                var credits = parseInt($.grep(srcStudySubjectIndex, function (obj) { return obj.Id == selected.params.data.id; })[0].Credits);
                srcCredits -= credits;
                $("#srcTotal").html("Total: " + srcCredits)
            });

            $("#trgStudySubjects").on("select2:select", function (selected) {
                var credits = parseInt($.grep(trgStudySubjectIndex, function (obj) { return obj.Id == selected.params.data.id; })[0].Credits);
                trgCredits += credits;
                $("#trgTotal").html("Total: " + trgCredits)
            });

            $("#trgStudySubjects").on("select2:unselect", function (selected) {
                var credits = parseInt($.grep(trgStudySubjectIndex, function (obj) { return obj.Id == selected.params.data.id; })[0].Credits);
                trgCredits -= credits;
                $("#trgTotal").html("Total: " + trgCredits)
            });
        }

    };
    var srcCredits = 0;
    var trgCredits = 0;
    var srcStudySubjectIndex = [];
    var trgStudySubjectIndex = [];

    //Private functions
    function disableSelectInput(target) {
        $(target).select2("val", "");
        $(target).attr('disabled', true);
    }

    function enableSelectInput(target) {
        $(target).attr('disabled', false);
    }

    function disableDefaultSelectInputs() {
        $("#srcFaculty").val(null).trigger("change");
        $("#trgUni").val(null).trigger("change");
        $("#srcFos").val(null).trigger("change");
        $("#trgFos").val(null).trigger("change");
        $("#trgFaculty").val(null).trigger("change");

        disableSelectInput("#srcFaculty");
        disableSelectInput("#trgUni");
        disableSelectInput("#trgFaculty");
        disableSelectInput("#srcFos");
        disableSelectInput("#trgFos");
        disableSelectInput("#semester");
        disableSelectInput("#srcStudySubjects");
        disableSelectInput("#trgStudySubjects");
    }

    //Registers component to global scope
    Erasmus.Student = student;
})(window.Erasmus = window.Erasmus || {}, jQuery);

//University agreement
(function (Erasmus, $, undefined) {
    var agreement = {
        Initialize : function() {
            $("#sourceUniversity").on("select2:select", function (selected) {
                $('#targetUniversity').empty();
                $.getJSON(Erasmus.Settings.UniversityAgreement.TargetUniversityUrl + "/?sourceUniversityId=" + selected.params.data.id, function (data) {
                    var dropdownData = [];
                    $.each(data, function (key, val) {
                        dropdownData.push({id:val.Key, text: val.Value});
                    });
                    $("#targetUniversity").select2({ data: dropdownData });
                    enableSelectInput("#targetUniversity");
                });
            });
        }
    };

    function enableSelectInput(target) {
        $(target).attr('disabled', false);
    }

    //Registers component to global scope
    Erasmus.UniversityAgreement = agreement;
})(window.Erasmus = window.Erasmus || {}, jQuery);

function ShowSaveResult(data) {
    if (data.type == "error") {
        changeModalClass("modal-danger");
    } else {
        changeModalClass("modal-success");
    }
    
    $("#modalBody").html(data.message);
    $("#modalResult").modal("show");

    function changeModalClass(newClass) {
        $("#modalResult").removeClass("modal-info");
        $("#modalResult").removeClass("modal-danger");
        $("#modalResult").removeClass("modal-warning");
        $("#modalResult").removeClass("modal-success");
        $("#modalResult").addClass(newClass);
    }
}
