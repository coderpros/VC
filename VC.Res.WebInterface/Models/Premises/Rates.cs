namespace VC.Res.WebInterface.Models.Premises.Rates
{
    public class Proposed
    {
        public Guid Id_Guid { get; private set; } = Guid.NewGuid();

        public DateTime Arrive { get; set; }
        public DateTime Depart { get; set; }
        public int MinPartySize { get; set; } = 0;

        public bool Price_POA { get; set; } = false;
        public Core.Enums.Shared_PriceValueType Price_EntryMode { get; set; } = Core.Enums.Shared_PriceValueType.Net;
        public decimal Price { get; set; } = 0;

        public bool Commission_SubjectTo { get; set; } = true;
        public Core.Enums.Shared_NumericValueType Commission_AmountType { get; set; } = Core.Enums.Shared_NumericValueType.Percentage;
        public decimal Commission_Amount { get; set; } = 0;
        public string Commission_Note { get; set; } = "";

        public bool Tax_Exempt { get; set; } = false;
        public decimal Tax_Value { get; set; } = 0;

        public bool Discount { get; set; } = false;
        public int Discount_Nights { get; set; } = 0;
        public Core.Enums.Shared_PriceValueType Discount_EntryMode { get; set; } = Core.Enums.Shared_PriceValueType.Gross;
        public Core.Enums.Shared_NumericValueType Discount_AmountType { get; set; } = Core.Enums.Shared_NumericValueType.Percentage;
        public decimal Discount_Amount { get; set; } = 0;
        public string Discount_Note { get; set; } = "";

        public decimal Price_Net { get; private set; }
        public decimal? Commission { get; private set; }
        public decimal? Tax { get; private set; }
        public decimal Price_Gross { get; private set; }

        public decimal? Editable_Price_Net { get; set; }
        public decimal? Editable_Commission { get; set; }
        public decimal? Editable_Tax { get; set; }
        public decimal? Editable_Price_Gross { get; set; }

        public Proposed() { }

        public void Calculate()
        {
            var objPriceCalculation = new Core.Models.PriceCalculation(
                Price_POA,
                Price_EntryMode,
                Price,
                Commission_SubjectTo,
                Commission_AmountType,
                Commission_Amount,
                Tax_Exempt,
                Tax_Value);

            Price_Net = objPriceCalculation.Price_Net;
            Commission = objPriceCalculation.Commission;
            Tax = objPriceCalculation.Tax;
            Price_Gross = objPriceCalculation.Price_Gross;

            Editable_Price_Net = Price_Net;
            Editable_Commission = Commission;
            Editable_Tax = Tax;
            Editable_Price_Gross = Price_Gross;

            if (Price_POA)
            {
                Editable_Price_Net = null;
                Editable_Commission = null;
                Editable_Tax = null;
                Editable_Price_Gross = null;
            }
        }

        public void Editable_Updated()
        {
            if (!Editable_Price_Gross.HasValue) { Editable_Price_Gross = 0; }
            if (!Editable_Price_Net.HasValue) { Editable_Price_Net = 0; }

            if (Editable_Price_Gross != Price_Gross)
            {
                // gross price has changed
                if (Editable_Price_Gross > 0)
                {
                    Price_POA = false;
                    Price_EntryMode = Core.Enums.Shared_PriceValueType.Gross;
                    Price = Editable_Price_Gross.Value;
                }
                else
                {
                    Price_POA = true;
                }
            }
            else if (Editable_Price_Net != Price_Net)
            {
                // net price has changed
                if (Editable_Price_Net > 0)
                {
                    Price_POA = false;
                    Price_EntryMode = Core.Enums.Shared_PriceValueType.Net;
                    Price = Editable_Price_Net.Value;
                }
                else
                {
                    Price_POA = true;
                }
            }

            if (!Editable_Tax.HasValue) { Editable_Tax = 0; }

            if (Editable_Tax != Tax)
            {
                // tax amount has changed
                if (Editable_Tax > 0)
                {
                    Tax_Exempt = false;
                    Tax_Value = Editable_Tax.Value;
                }
                else
                {
                    Tax_Exempt = true;
                }
            }

            if (!Editable_Commission.HasValue) { Editable_Commission = 0; }
            if (Editable_Commission != Commission)
            {
                // commission has changed
                if (Editable_Commission > 0)
                {
                    Commission_SubjectTo = true;
                    if (Editable_Commission <= 100)
                    {
                        // treat as a percentage
                        Commission_AmountType = Core.Enums.Shared_NumericValueType.Percentage;
                    }
                    else
                    {
                        // treat as an actual value
                        Commission_AmountType = Core.Enums.Shared_NumericValueType.Fixed;
                    }
                    Commission_Amount = Editable_Commission.Value;
                }
                else
                {
                    Commission_SubjectTo = false;
                }
            }

            Calculate();
        }
    }

    public class Summary
    {
        public int Id { get; private set; }

        public Core.Premises.Seasons.Season Season { get; private set; }

        public Core.Common.Currency Currency { get; private set; }

        public bool Provisional { get; private set; } = false;
        public bool RequireReview { get; private set; } = false;

        public DateTime Arrive { get; private set; }

        public DateTime Depart { get; private set; }

        public int NoNights { get; private set; }

        public int MinPartySize { get; private set; }

        public bool Price_POA { get; private set; }
        public Core.Enums.Shared_PriceValueType Price_EntryMode { get; private set; }
        public decimal Price { get; private set; }

        public bool Commission_SubjectTo { get; private set; }
        public Core.Enums.Shared_NumericValueType Commission_AmountType { get; private set; }
        public decimal Commission_Amount { get; private set; }

        public bool Tax_Exempt { get; private set; }
        public decimal Tax_Value { get; private set; }


        public decimal Price_Net { get; private set; }

        public decimal? Commission { get; private set; }

        public decimal? Tax { get; private set; }

        public decimal Price_Gross { get; private set; }


        public Core.Enums.Premises_Premise_Availability Availability { get; private set; } = Core.Enums.Premises_Premise_Availability.Unknown;


        public DateTime Editable_Arrive { get; set; }
        public DateTime Editable_Depart { get; set; }
        public decimal? Editable_Price_Net { get; set; }
        public decimal? Editable_Commission { get; set; }
        public decimal? Editable_Price_Gross { get; set; }
        public Core.Enums.Premises_Premise_Availability Editable_Availability { get; set; }


        public Summary() { }


        public async Task Load(Core.Premises.Seasons.Rate objRate)
        {
            Id = objRate.Id;

            if (objRate.Season_Id.HasValue)
            {
                Season = await Core.Premises.Seasons.Season.FindAsync(objRate.Season_Id.Value);
                Currency = await Core.Common.Currency.FindAsync((await Core.Premises.Config.FindBy_SeasonAsync(Season.Id)).Currency_Id_Calculated);
            }
            else
            {
                Season = new Core.Premises.Seasons.Season();
                Currency = await Core.Common.Currency.FindAsync(Core.Settings.PremiseDefaults.Fetch.Currency_Id);
            }

            Provisional = objRate.Provisional;
            RequireReview = objRate.RequireReview;

            Arrive = objRate.Arrive;
            Depart = objRate.Depart;

            NoNights = objRate.No_Nights;
            MinPartySize = objRate.Min_PartySize;

            Price_POA = objRate.Price_POA;
            Price_EntryMode = objRate.Price_EntryMode;
            Price = objRate.Price;

            Commission_SubjectTo = true;
            Commission_AmountType = objRate.Commission_AmountType;
            Commission_Amount = objRate.Commission_Amount;

            Tax_Exempt = objRate.Tax_Exempt;
            Tax_Value = objRate.Tax_Value;

            var objPriceCalculation = new Core.Models.PriceCalculation(
                Price_POA,
                Price_EntryMode,
                Price,
                true,
                Commission_AmountType,
                Commission_Amount,
                Tax_Exempt,
                Tax_Value);

            Price_Net = objPriceCalculation.Price_Net;
            Commission = objPriceCalculation.Commission;
            Tax = objPriceCalculation.Tax;
            Price_Gross = objPriceCalculation.Price_Gross;

            // get the availabity for the period
            var lstAvailability = await Core.Premises.Availability.FindAllBy_PremiseDatesAsync(objRate.Premise_Id, Arrive, Depart.AddDays(-1));

            if (lstAvailability.All(r => r.State == Core.Enums.Premises_Premise_Availability.Available))
            {
                Availability = Core.Enums.Premises_Premise_Availability.Available;
            }
            else if (lstAvailability.All(r => r.State == Core.Enums.Premises_Premise_Availability.AvailableEnquire))
            {
                Availability = Core.Enums.Premises_Premise_Availability.AvailableEnquire;
            }
            else if (lstAvailability.All(r => r.State == Core.Enums.Premises_Premise_Availability.Booked))
            {
                Availability = Core.Enums.Premises_Premise_Availability.Booked;
            }
            else if (lstAvailability.All(r => r.State == Core.Enums.Premises_Premise_Availability.BookedExt))
            {
                Availability = Core.Enums.Premises_Premise_Availability.BookedExt;
            }
            else if (lstAvailability.All(r => r.State == Core.Enums.Premises_Premise_Availability.OnHold))
            {
                Availability = Core.Enums.Premises_Premise_Availability.OnHold;
            }
            else if (lstAvailability.All(r => r.State == Core.Enums.Premises_Premise_Availability.Unavailable))
            {
                Availability = Core.Enums.Premises_Premise_Availability.Unavailable;
            }

            lstAvailability = null;

            Editable_Arrive = Arrive;
            Editable_Depart = Depart;
            Editable_Price_Net = Price_Net;
            Editable_Commission = Commission;
            Editable_Price_Gross = Price_Gross;
            Editable_Availability = Availability;

            if (Price_POA)
            {
                Editable_Price_Net = null;
                Editable_Commission = null;
                Editable_Price_Gross = null;
            }
        }

        public async Task<bool> Save(string strBy)
        {
            if (Id < 1) { return false; }

            // get the rate to update
            var objRate = await Core.Premises.Seasons.Rate.FindAsync(Id, bUseCache: false);

            if (!objRate.Loaded || objRate.Deleted_UTC.HasValue) { return false; }

            var bChanged = false;

            if (Editable_Arrive != objRate.Arrive)
            {
                // changed arrival date
                Arrive = objRate.Arrive;
                objRate.Arrive = Editable_Arrive;

                bChanged = true;
            }

            if (Editable_Depart != objRate.Depart)
            {
                // changed departure date
                Depart = objRate.Depart;
                objRate.Depart = Editable_Depart;

                bChanged = true;
            }

            if (!Editable_Price_Gross.HasValue) { Editable_Price_Gross = 0; }
            if (!Editable_Price_Net.HasValue) { Editable_Price_Net = 0; }

            if (Editable_Price_Gross != Price_Gross)
            {
                // gross price has changed
                if (Editable_Price_Gross > 0)
                {
                    objRate.Price_POA = false;
                    objRate.Price_EntryMode = Core.Enums.Shared_PriceValueType.Gross;
                    objRate.Price = Editable_Price_Gross.Value;

                    bChanged = true;
                }
                else
                {
                    objRate.Price_POA = true;

                    bChanged = true;
                }
            }
            else if (Editable_Price_Net != Price_Net)
            {
                // net price has changed
                if (Editable_Price_Net > 0)
                {
                    objRate.Price_POA = false;
                    objRate.Price_EntryMode = Core.Enums.Shared_PriceValueType.Net;
                    objRate.Price = Editable_Price_Net.Value;

                    bChanged = true;
                }
                else
                {
                    objRate.Price_POA = true;

                    bChanged = true;
                }
            }

            if (bChanged)
            {
                if (!await objRate.SaveAsync(strBy))
                {
                    return false;
                }
            }

            if (Availability != Editable_Availability && Availability != Core.Enums.Premises_Premise_Availability.Unknown)
            {
                // availability has changed
                // check the availability we're going to change is still the same as originally
                if ((await Core.Premises.Availability.FindAllBy_PremiseDatesAsync(objRate.Premise_Id, objRate.Arrive, objRate.Depart.AddDays(-1))).All(r => r.State == Availability))
                {
                    // ok to change
                    _ = await Core.Premises.Availability.UpdateAsync(objRate.Premise_Id, Editable_Availability, objRate.Arrive, objRate.Depart.AddDays(-1), strBy: strBy);
                }
            }

            if (objRate.Season_Id.HasValue && (Editable_Arrive != Arrive || Editable_Depart != Depart))
            {
                // one of the rates dates has changed, process the other rates to move arrival/departure dates accordingly (automatically)
                var lstRates = await Core.Premises.Seasons.Rate.FindAllBy_SeasonAsync(objRate.Season_Id.Value);

                foreach (var objSeasonRate in lstRates)
                {
                    if (objSeasonRate.Id == objRate.Id) { continue; }

                    var bSeasonRateChanged = false;

                    // work out current number of nights incase it changes
                    var iNights = objSeasonRate.No_Nights;

                    // the arrival date changed, update any other rates for this season that depart on the old arrival date, to now be the new arrival date
                    if (objSeasonRate.Depart == Arrive && Editable_Arrive != Arrive)
                    {
                        objSeasonRate.Depart = Editable_Arrive.Date;

                        bSeasonRateChanged = true;
                    }

                    // the departure date changed, update any other rates for this season that arrive on the old departure date, to now be the new departure date.
                    if (objSeasonRate.Arrive == Depart && Editable_Depart != Depart)
                    {
                        objSeasonRate.Arrive = Editable_Depart.Date;

                        bSeasonRateChanged = true;
                    }

                    // check if number of nights has changed
                    var iNewNoNights = (objSeasonRate.Depart - objSeasonRate.Arrive).Days;

                    if (iNewNoNights != iNights)
                    {
                        // check it is still above the minimum
                        var objConfig = objSeasonRate.Season_Id.HasValue ? await Core.Premises.Config.FindBy_SeasonAsync(objSeasonRate.Season_Id.Value) : await Core.Premises.Config.FindBy_PremiseAsync(objSeasonRate.Premise_Id);
                        if (iNewNoNights < objConfig.MinRental_Days_Calculated)
                        {
                            // can't allow the change
                            bSeasonRateChanged = false;
                        }
                        objConfig = null;

                        // try to auto adjust price and commission
                        if (objSeasonRate.Price > 0)
                        {
                            // workout previous nightly price
                            var dNightlyPrice = objSeasonRate.Price / iNights;

                            // use previous nightly to work out a new price for the rate
                            objSeasonRate.Price = dNightlyPrice * (objSeasonRate.Depart - objSeasonRate.Arrive).Days;
                        }

                        if (objSeasonRate.Commission_AmountType == Core.Enums.Shared_NumericValueType.Fixed && objSeasonRate.Commission_Amount > 0)
                        {
                            // work out a new commission amount using previous nights to workout a nightly price
                            var dNightlyPrice = objSeasonRate.Commission_Amount / iNights;

                            objSeasonRate.Commission_Amount = dNightlyPrice * (objSeasonRate.Depart - objSeasonRate.Arrive).Days;
                        }
                    }

                    if (bSeasonRateChanged)
                    {
                        // we've made automatic changes to the rate so flag to be reviewed if not already provisional
                        if (!objSeasonRate.Provisional) { objSeasonRate.RequireReview = true; }

                        _ = await objSeasonRate.SaveAsync(strBy);
                    }
                }

                lstRates = null;
            }

            return true;
        }
    }
}
