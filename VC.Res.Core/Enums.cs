namespace VC.Res.Core
{
    public class Enums
    {
        public enum Shared_AgentPaymentPoint
        {
            Unknown = 0,
            Staged = 10,
            Deposit = 20,
            Interim = 30,
            Balance = 40,
            Arrival = 50,
            Departure = 60
        };

        public enum Shared_NumericValueType
        {
            Unknown = 0,
            Percentage = 10,
            Fixed = 20
        };

        // Net = not including tax/commission etc
        // Gross = Total price to be paid including all taxes and commissions etc
        public enum Shared_PriceValueType
        {
            Unknown = 0,
            Net = 10,
            Gross = 20
        };

        public enum Shared_TwoFAMethod
        {
            Disabled = 0,
            Email = 10,
            MobileText = 20
        };

        public enum Common_Tag_Type
        {
            Unknown = 0,
            ContactService = 10,
            PropertyFeature = 20
        };

        public enum Contacts_Contact_Category
        {
            Owner = 10,
            Agent = 20,
            ExclusiveAgent = 30,
            VillaAdmin = 40,
            VillaManager = 50,
            ReservationManager = 60,
            ReservationTeam = 70,
            ManagementCompany = 80,
            DestinationVillaSpecialist = 90,
            GroundHandler = 100,
            Customer = 110,
            Other = 0
        };

        public enum Contacts_Contact_PreferredContactMethod
        {
            Unknown = 0,
            Email = 10,
            Phone = 20,
            WhatsApp = 30,
            Text = 40
        };

        public enum Premises_Premise_Availability
        {
            Unknown = 0,
            Available = 10,
            AvailableEnquire = 20,
            Unavailable = 30,
            OnHold = 40,
            Booked = 50,
            BookedExt = 60
        };

        public enum Premises_Premise_Channel
        {
            Unknown = 0,
            DirectOnsite = 10,
            DirectOffsite = 20,
            AgentOnsite = 30,
            AgentOffsite = 40
        };

        public enum Premises_Related_Type
        {
            Unknown = 0,
            Alternative = 10,
            RentTogether = 20
        };

        public enum Users_Activity_ActionGroup
        {
            Unknown = 0,
            Authentication = 100,
            Setting = 600,
            User = 800
        };

        public enum Users_Activity_ActionType
        {
            Unknown = 0,
            Login = 10,
            Access = 20,
            View = 30,
            Add = 40,
            Edit = 50,
            Delete = 60,
            Download = 70,
            Request = 80,
            Attempt = 90
        };

        public enum Users_Session_Type
        {
            Web = 0
        };

        public enum Utilities_Audit_Action
        {
            Unknown = 1,
            Add = 2,
            Update = 3,
            Delete = 4,
            Undelete = 5
        };

        public enum Utilities_Email_Type
        {
            Unknown = 0,
            User_PasswordSet = 100,
            User_PasswordReset = 110,
            User_AuthCode = 120
        };

        public enum Utilities_SortOption_Order
        {
            Asc,
            Desc
        };

        public enum Workflows_Workflow_Type
        {
            Unknown = 0,
            Booking = 10
        };

        public enum Workflows_Item_AutoCalcElement
        {
            NotApplicable = 0,
            Booking_Date_Made = 100,
            Booking_Date_Arrival = 110,
            Booking_Date_Departure = 120,
            Booking_Deposit_Due = 200,
            Booking_Deposit_Paid = 210,
            Booking_Interim_Due = 300,
            Booking_Interim_Paid = 310,
            Booking_Balance_Due = 400,
            Booking_Balance_Paid = 410,
            Booking_SecurityDeposit_Due = 500,
            Booking_SecurityDeposit_Paid = 510
        };

        public enum Workflows_Item_AutoUpdateElement
        {
            NotApplicable = 0,
            Booking_Deposit_Required = 100,
            Booking_Deposit_NotRequired = 110,
            Booking_Deposit_Paid = 120,
            Booking_Interim_Required = 200,
            Booking_Interim_NotRequired = 210,
            Booking_Interim_Paid = 220,
            Booking_Balance_Paid = 320,
            Booking_SecurityDeposit_Required = 400,
            Booking_SecurityDeposit_NotRequired = 410,
            Booking_SecurityDeposit_Paid = 420
        };


        public static string Label(Enum enumValue)
        {
            // We only need to add elements where the enum text isn't right, for example when a space is required or it needs to be named something else
            return enumValue switch
            {
                Shared_TwoFAMethod.Disabled => "None",
                Shared_TwoFAMethod.MobileText => "Text message",

                Common_Tag_Type.ContactService => "Contact service",
                Common_Tag_Type.PropertyFeature => "Property feature",

                Contacts_Contact_Category.ExclusiveAgent => "Exclusive agent",
                Contacts_Contact_Category.VillaAdmin => "Villa admin",
                Contacts_Contact_Category.VillaManager => "Villa manager",
                Contacts_Contact_Category.ReservationManager => "Reservation manager",
                Contacts_Contact_Category.ReservationTeam => "Reservation team",
                Contacts_Contact_Category.ManagementCompany => "Management company",
                Contacts_Contact_Category.DestinationVillaSpecialist => "Destination villa specialist",
                Contacts_Contact_Category.GroundHandler => "Ground handler",

                Contacts_Contact_PreferredContactMethod.Unknown => "Not specified",

                Premises_Premise_Availability.AvailableEnquire => "Available - Enquire",
                Premises_Premise_Availability.OnHold => "On hold",
                Premises_Premise_Availability.BookedExt => "Booked - Externally",

                Premises_Related_Type.RentTogether => "Rent together",

                Workflows_Item_AutoCalcElement.NotApplicable => "Not applicable",
                Workflows_Item_AutoCalcElement.Booking_Date_Made => "Booking - Date made/confirmed",
                Workflows_Item_AutoCalcElement.Booking_Date_Arrival => "Booking - Arrival date",
                Workflows_Item_AutoCalcElement.Booking_Date_Departure => "Booking - Departure date",
                Workflows_Item_AutoCalcElement.Booking_Deposit_Due => "Booking - Deposit due",
                Workflows_Item_AutoCalcElement.Booking_Deposit_Paid => "Booking - Deposit paid",
                Workflows_Item_AutoCalcElement.Booking_Interim_Due => "Booking - Interim due",
                Workflows_Item_AutoCalcElement.Booking_Interim_Paid => "Booking - Interim paid",
                Workflows_Item_AutoCalcElement.Booking_Balance_Due => "Booking - Balance due",
                Workflows_Item_AutoCalcElement.Booking_Balance_Paid => "Booking - Balance paid",
                Workflows_Item_AutoCalcElement.Booking_SecurityDeposit_Due => "Booking - Security deposit due",
                Workflows_Item_AutoCalcElement.Booking_SecurityDeposit_Paid => "Booking - Security deposit paid",

                Workflows_Item_AutoUpdateElement.NotApplicable => "Not applicable",
                Workflows_Item_AutoUpdateElement.Booking_Deposit_Required => "Booking - Deposit required",
                Workflows_Item_AutoUpdateElement.Booking_Deposit_NotRequired => "Booking - Deposit not required",
                Workflows_Item_AutoUpdateElement.Booking_Deposit_Paid => "Booking - Deposit paid",
                Workflows_Item_AutoUpdateElement.Booking_Interim_Required => "Booking - Interim required",
                Workflows_Item_AutoUpdateElement.Booking_Interim_NotRequired => "Booking - Interim not required",
                Workflows_Item_AutoUpdateElement.Booking_Interim_Paid => "Booking - Interim paid",
                Workflows_Item_AutoUpdateElement.Booking_Balance_Paid => "Booking - Balance paid",
                Workflows_Item_AutoUpdateElement.Booking_SecurityDeposit_Required => "Booking - Security deposit required",
                Workflows_Item_AutoUpdateElement.Booking_SecurityDeposit_NotRequired => "Booking - Security deposit not required",
                Workflows_Item_AutoUpdateElement.Booking_SecurityDeposit_Paid => "Booking - Security deposit paid",

                Premises_Premise_Channel.Unknown => "Not available",
                Premises_Premise_Channel.DirectOnsite => "Direct onsite",
                Premises_Premise_Channel.DirectOffsite => "Direct offsite",
                Premises_Premise_Channel.AgentOnsite => "Agent onsite",
                Premises_Premise_Channel.AgentOffsite => "Agent offsite",

                _ => Shared.Enums.Label(enumValue),
            };
        }

        public static string Label_Plural(Enum enumValue)
        {
            // We only need to add elements where the enum text isn't right, for example when a space is required or it needs to be named something else
            return enumValue switch
            {
                Common_Tag_Type.ContactService => "Contact services",
                Common_Tag_Type.PropertyFeature => "Property features",

                Contacts_Contact_Category.Owner => "Owners",
                Contacts_Contact_Category.Agent => "Agents",
                Contacts_Contact_Category.ExclusiveAgent => "Exclusive agents",
                Contacts_Contact_Category.VillaAdmin => "Villa admins",
                Contacts_Contact_Category.VillaManager => "Villa managers",
                Contacts_Contact_Category.ReservationManager => "Reservation managers",
                Contacts_Contact_Category.ReservationTeam => "Reservation teams",
                Contacts_Contact_Category.ManagementCompany => "Management companies",
                Contacts_Contact_Category.DestinationVillaSpecialist => "Destination villa specialists",
                Contacts_Contact_Category.GroundHandler => "Ground handlers",
                Contacts_Contact_Category.Customer => "Customers",

                Premises_Related_Type.Alternative => "Alternatives",
                Premises_Related_Type.RentTogether => "Rent together",

                _ => Shared.Enums.Label_Plural(enumValue),
            };
        }
    }
}
