(function () {
    'use strict';

    window.EducationAccelerator = window.EducationAccelerator || {};
    window.EducationAccelerator.Forms = window.EducationAccelerator.Forms || {};
    window.EducationAccelerator.Forms.TestScore = (function () {

        var formContext;
        var testTypeControl;

        var TAB_NAME = '{455114cb-24aa-4715-9f74-f0c8a47b84c8}';
        var ALL_SECTIONS = [
            'act_section',
            'gmat_section',
            'gre_section',
            'ielts_section',
            'sat_section',
            'toefl_section'
        ];

        function executeOnLoad(executionContext) {
            if (!executionContext) {
                return;
            }

            formContext = executionContext.getFormContext();

            testTypeControl = formContext.getControl('mshied_testtypeid'); // Lookup
            setupTestTypeOnChange();
        }

        function setupTestTypeOnChange() {
            if (!testTypeControl) {
                return;
            }

            var testTypeAttribute = testTypeControl.getAttribute();
            if (!testTypeAttribute) {
                return;
            }

            // Run initial check
            onTestTypeChange();

            testTypeAttribute.addOnChange(onTestTypeChange);
        }

        function onTestTypeChange() {
            hideSections(TAB_NAME, ALL_SECTIONS);

            var testTypeName;
            var testTypeValue = testTypeControl.getAttribute().getValue();
            if (testTypeValue && testTypeValue.length > 0 && testTypeValue[0].name) {
                testTypeName = testTypeValue[0].name.toUpperCase();
            }

            switch (testTypeName) {
                case 'ACT':
                    showSection(TAB_NAME, 'act_section');
                    break;
                case 'GMAT':
                    showSection(TAB_NAME, 'gmat_section');
                    break;
                case 'GRE':
                    showSection(TAB_NAME, 'gre_section');
                    break;
                case 'IELTS':
                    showSection(TAB_NAME, 'ielts_section');
                    break;
                case 'SAT':
                    showSection(TAB_NAME, 'sat_section');
                    break;
                case 'TOEFL':
                    showSection(TAB_NAME, 'toefl_section');
                    break;
                default:
                    showSections(TAB_NAME, ALL_SECTIONS);
                    break;
            }
        }

        function hideSections(tabName, sectionNames) {
            if (!Array.isArray(sectionNames)) {
                return;
            }

            for (var sectionName of sectionNames) {
                EducationAccelerator.Utilities.FormUtil.hideSection(formContext, tabName, sectionName);
            }
        }

        function showSections(tabName, sectionNames) {
            if (!Array.isArray(sectionNames)) {
                return;
            }

            for (var sectionName of sectionNames) {
                EducationAccelerator.Utilities.FormUtil.showSection(formContext, tabName, sectionName);
            }
        }

        function showSection(tabName, sectionName) {
            EducationAccelerator.Utilities.FormUtil.showSection(formContext, tabName, sectionName);
        }

        return {
            executeOnLoad: executeOnLoad
        };
    }());
}());
