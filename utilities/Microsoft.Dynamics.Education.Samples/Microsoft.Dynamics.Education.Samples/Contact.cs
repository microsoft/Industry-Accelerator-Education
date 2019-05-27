// =====================================================================
//  This file is part of the Microsoft Dynamics Accelerator code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Microsoft.Dynamics.Education.Samples
{
    public class Contact
    {

        #region Class Level Members

        /// <summary>
        /// Stores the organization service proxy.
        /// </summary>
        private OrganizationServiceProxy _serviceProxy;

        #endregion Class Level Members

        #region How To Sample Code
        /// <summary>
        /// Create and configure the organization service proxy.
        /// Initiate the method to create any data that this sample requires.
        /// Create an appointment.
        /// </summary>
        public void Run(ServerConnection.Configuration serverConfig, bool promptforDelete)
        {
            try
            {
                //<snippetMarketingAutomation1>
                // Connect to the Organization service. 
                // The using statement assures that the service proxy will be properly disposed.
                using (_serviceProxy = new OrganizationServiceProxy(serverConfig.OrganizationUri, serverConfig.HomeRealmUri, serverConfig.Credentials, serverConfig.DeviceCredentials))
                {
                    // This statement is required to enable early-bound type support.
                    _serviceProxy.EnableProxyTypes();

					var contact = new Entity("contact");

					contact["mshied_currentprogramlevelid"] = new EntityReference("mshied_programlevel", new Guid("f08ceedd-64c6-e811-a984-000d3a1618d5"));
					contact["mshied_currentprogramid"] = new EntityReference("mshied_program", new Guid("6d0d24af-f1cb-e811-a99d-000d3a161343"));
					contact["mshied_currentacademicperiodid"] = new EntityReference("mshied_academicperiod", new Guid("365f7238-64c6-e811-a98e-000d3a1612a7"));
					contact["birthdate"] = new DateTime(1997, 4, 14);
                    contact["mshied_firstgeneration"] = false;
					contact["mshied_ferpaprivacy"] = false;
					contact["msdyn_gdproptout"] = false;
					contact["mshied_hipaaindicator"] = false;
					contact["mshied_lastpermanentresidencecountry"] = new OptionSetValue(494280002); // USA
					contact["mshied_legacy"] = false;
					contact["mshied_manualriskscore"] = string.Empty;
					contact["mshied_miltarystatus"] = new OptionSetValue(494280000); // No Military Service
					contact["mshied_nationalidentifier"] = string.Empty;
					contact["mshied_race"] = new OptionSetValue(494280000); // Race 1
					contact["mshied_studentstatusid"] = new EntityReference("mshied_studentstatus", new Guid("345e53be-f3cb-e811-a988-000d3a1618d5"));
					contact["firstname"] = "Vivian";
					contact["lastname"] = "Gonzales";
					contact["emailaddress1"] = "Vivian.Gonzales@example.com";
					contact["gendercode"] = new OptionSetValue(2); // Female
					contact["address1_addresstypecode"] = new OptionSetValue(1); // Bill To
					contact["address1_city"] = "Denver";
					contact["address1_country"] = "USA";
					contact["address1_stateorprovince"] = "CO";
					contact["address1_line1"] = "724 Ratke Walk";
					contact["address1_line2"] = "7B";
					contact["address1_postalcode"] = "80802";

					var id = _serviceProxy.Create(contact);

					// Verify that the record has been created.
                    if (id != Guid.Empty)
                    {
                        Console.WriteLine($"Successfully created {id}.");
                    }
                }
            }
            // Catch any service fault exceptions that Microsoft Dynamics CRM throws.
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
            {
                // You can handle an exception here or pass it back to the calling method.
                throw;
            }
        }

        #endregion How To Sample Code       


        #region Main Method

        /// <summary>
        /// Standard Main() method used by most SDK samples.
        /// </summary>
        /// <param name="args"></param>
        static public void Main(string[] args)
        {
            try
            {
                // Obtain the target organization's Web address and client logon 
                // credentials from the user.
                ServerConnection serverConnect = new ServerConnection();
                ServerConnection.Configuration config = serverConnect.GetServerConfiguration();

                var app = new Contact();
                app.Run(config, true);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Plugin Trace: {0}", ex.Detail.TraceText);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (System.TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
                    if (fe != null)
                    {
                        Console.WriteLine($"Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine($"Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine($"Message: {0}", fe.Detail.Message);
                        Console.WriteLine($"Plugin Trace: {0}", fe.Detail.TraceText);
                        Console.WriteLine($"Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }
            // Additional exceptions to catch: SecurityTokenValidationException, ExpiredSecurityTokenException,
            // SecurityAccessDeniedException, MessageSecurityException, and SecurityNegotiationException.

            finally
            {
                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
        #endregion Main method

    }
}
