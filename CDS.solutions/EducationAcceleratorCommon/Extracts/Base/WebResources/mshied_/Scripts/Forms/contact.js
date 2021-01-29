(function () {
    'use strict';

    window.EducationAccelerator = window.EducationAccelerator || {};
    window.EducationAccelerator.Forms = window.EducationAccelerator.Forms || {};
    window.EducationAccelerator.Forms.Contact = (function () {

        var formContext;
        var contactTypeControl;

        var pageStorage = window.sessionStorage;
        var SESSION_PREVIOUS_FORM_KEY = 'EducationAcceleratorPreviousFormName';

        // Option set values
        var ADVISOR_CONTACT_TYPE = 494280006;
        var INSTRUCTOR_CONTACT_TYPE = 494280000;
        var FACULTY_CONTACT_TYPE = 494280003;
        var STAFF_CONTACT_TYPE = 494280010;
        var STUDENT_CONTACT_TYPE = 494280011;

        function executeOnLoad(executionContext) {
            if (!executionContext) {
                return;
            }

            formContext = executionContext.getFormContext();

            contactTypeControl = formContext.getControl('mshied_contacttype'); // Lookup
            setupContactTypeOnChange();

            // Comparing the previous form loaded vs current form loading now, we can determine if a user (or contact type trigger) is changing the form
            // If this is the case, we do not try to force the correct form.
            // If the form name matches, or there is no previous form name, we can assume the user has just opened the record or refreshed,
            // and we want to force the correct form.
            var previousFormId = pageStorage.getItem(SESSION_PREVIOUS_FORM_KEY);
            var currentForm = formContext.ui.formSelector.getCurrentItem();
            if (currentForm) {
                pageStorage.setItem(SESSION_PREVIOUS_FORM_KEY, currentForm.getId());
            }
            if (!previousFormId || (currentForm && previousFormId === currentForm.getId())) {
                onContactTypeChange();
            }
        }

        function setupContactTypeOnChange() {
            if (!contactTypeControl) {
                return;
            }

            var contactTypeAttribute = contactTypeControl.getAttribute();
            if (!contactTypeAttribute) {
                return;
            }

            contactTypeAttribute.addOnChange(onContactTypeChange);
        }

        function onContactTypeChange() {
            // No value? No operation
            var contactTypeValues = contactTypeControl.getAttribute().getValue();
            if (!contactTypeValues || contactTypeValues.length === 0) {
                return;
            }

            switch (contactTypeValues[0]) {
                case ADVISOR_CONTACT_TYPE:
                case INSTRUCTOR_CONTACT_TYPE:
                case FACULTY_CONTACT_TYPE:
                case STAFF_CONTACT_TYPE:
                    // Higher Education Faculty Form
                    EducationAccelerator.Utilities.FormUtil.navigateToForm(formContext, '94eb6fcd-3aa2-4b1f-9eea-f23da74df76f');
                    break;
                case STUDENT_CONTACT_TYPE:
                    // Higher Education Student Form
                    EducationAccelerator.Utilities.FormUtil.navigateToForm(formContext, 'dc419af9-8994-4f86-91db-8d0169c0ad4e');
                    break;
            }
        }

        return {
            executeOnLoad: executeOnLoad
        };
    }());
}());
