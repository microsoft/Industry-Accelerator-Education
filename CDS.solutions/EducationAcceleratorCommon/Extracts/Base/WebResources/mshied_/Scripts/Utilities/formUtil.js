(function () {
    'use strict';

    window.EducationAccelerator = window.EducationAccelerator || {};
    window.EducationAccelerator.Utilities = window.EducationAccelerator.Utilities || {};
    window.EducationAccelerator.Utilities.FormUtil = (function () {

        function navigateToForm(formContext, formId) {
            if (!formContext || !formContext.ui) {
                return;
            }

            var currentForm = formContext.ui.formSelector.getCurrentItem();
            // Null if only one form avaialble, so navigate isn't needed
            if (!currentForm) {
                return;
            }


            var navigateToForm = formContext.ui.formSelector.items.get(formId);
            if (!navigateToForm) {
                return;
            }

            // Trying to naviaget to the already selected form? skip
            if (currentForm.getLabel() === navigateToForm.getLabel()) {
                return;
            }

            navigateToForm.navigate();
        }
	
        function showSection(formContext, tabName, sectionName) {
            showHideSection(formContext, tabName, sectionName, true);
        }
		
        function hideSection(formContext, tabName, sectionName) {
            showHideSection(formContext, tabName, sectionName, false);
        }

        function showHideSection(formContext, tabName, sectionName, show) {
            if (!formContext || !formContext.ui) {
                return;
            }

            var tab = formContext.ui.tabs.get(tabName);
            if (!tab) {
                return;
            }

            var section = tab.sections.get(sectionName);
            if (!section) {
                return;
            }

            section.setVisible(show);
        }

        return {
            navigateToForm: navigateToForm,
            showSection: showSection,
            hideSection: hideSection
        };
    }());
}());
