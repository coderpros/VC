using Syncfusion.Blazor.Grids;
using System.Text;
using VC.Res.Core;

namespace VC.Res.WebInterface.Models.Quoting
{
    public enum ItemType
    {
        Unknown,
        Date,
        Extra
    }

    public class Container
    {
        public DateTime Arrival { get; set; } = DateTime.UtcNow;
        public DateTime Departure { get; set; } = DateTime.UtcNow.AddDays(7);
        public bool Flexible_Arrival { get; set; } = false;
        public bool Include_Unavailable { get; set; } = false;
        public bool Include_Minimum { get; set; } = false;
        public int Number_People { get; set; } = 0;
        public List<int> Features { get; set; } = new List<int>();
        public List<Quote> Quotes { get; set; } = new List<Quote>();
        public Core.Common.Currency Currency { get; set; } = new Core.Common.Currency();
        public decimal Total_Net
        {
            get
            {
                return Quotes.Select(x => x.Total_Net).Sum();
            }
        }

        public decimal Total_Commission
        {
            get
            {
                return Quotes.Select(x => x.Total_Commission).Sum();
            }
        }

        public decimal Total_Tax
        {
            get
            {
                return Quotes.Select(x => x.Total_Tax).Sum();
            }
        }

        public decimal Total_Gross
        {
            get
            {
                return Quotes.Select(x => x.Total_Gross).Sum();
            }
        }

        public async Task<Container> GenerateQuote(Container objContainer, Core.Premises.Premise objPremise, Core.Common.Currency objCurrency, DateTime dtArrival, DateTime dtSecondary, bool bFlexiableArrival = false, int? iNumGuests = null)
        {
            var tmpQuote = new Quote();

            if (bFlexiableArrival)
            {
                //if flexi arrival, need to find all rates within date range first
                var dtSelectedDate = dtArrival;
                var lstSelectedRates = new List<Core.Premises.Seasons.Rate>();

                while (dtSelectedDate.Date < dtSecondary.Date)
                {
                    var objRate = (await Core.Premises.Seasons.Rate.FindAllBy_PremiseAsync(objPremise.Id, dtIncludes: dtSelectedDate.Date, iPartySize: iNumGuests)).FirstOrDefault();

                    if (objRate != null && objRate.Loaded)
                    {
                        if (!lstSelectedRates.Contains(objRate))
                        {
                            lstSelectedRates.Add(objRate);
                        }
                    }

                    dtSelectedDate = dtSelectedDate.AddDays(1);
                }


                //now build a quote for each rate for the premise
                if (lstSelectedRates.Count > 0)
                {
                    foreach (var objRate in lstSelectedRates)
                    {

                        var dtStartDate = objRate.Arrive;

                        if (dtArrival.Date > objRate.Arrive && dtArrival < objRate.Depart)
                        {
                            dtStartDate = dtArrival;
                        }

                        tmpQuote = await tmpQuote.BuildQuote(objPremise, objCurrency, dtStartDate, objRate.Depart, iNumGuests);

                        objContainer.Quotes.Add(tmpQuote);
                    }
                }
                else
                {
                    //no rates found so will have to use defaults
                    //as no departure date is set, will just have to get a weekly price as default
                    var dtDepart = dtArrival.AddDays(7);

                    tmpQuote = await tmpQuote.BuildQuote(objPremise, objCurrency, dtArrival, dtDepart, iNumGuests);

                    objContainer.Quotes.Add(tmpQuote);
                }
            }
            else
            {
                //build quote based on the selected premise and dates
                tmpQuote = await tmpQuote.BuildQuote(objPremise, objCurrency, dtArrival, dtSecondary, iNumGuests);

                objContainer.Quotes.Add(tmpQuote);
            }

            return objContainer;
        }
    }

    public class Quote
    {
        public int Id { get; set; } = 0;
        public Core.Premises.Premise Premises { get; set; } = new Core.Premises.Premise();
        public DateTime Arrival { get; set; } = DateTime.UtcNow;
        public DateTime Departure { get; set; } = DateTime.UtcNow.AddDays(7);
        public Core.Common.Currency Currency { get; set; } = new Core.Common.Currency();
        public List<Item> Items { get; set; } = new List<Item>();
        public string Snippets { get; set; } = string.Empty;
        public bool Selected { get; set; } = false;
        public Core.Enums.Premises_Premise_Availability Availability { get; set; } = Core.Enums.Premises_Premise_Availability.Unknown;

        public string Snippet { get; set; } = "";
        public decimal Accomodation_Net
        {
            get
            {
                return Items.Where(x => x.Type == ItemType.Date).Select(x => x.Price_Net).Sum();
            }
        }

        public decimal Accomodation_Commission
        {
            get
            {
                return Items.Where(x => x.Type == ItemType.Date).Select(x => x.Commission).Sum();
            }
        }

        public decimal Accomodation_Tax
        {
            get
            {
                return Items.Where(x => x.Type == ItemType.Date).Select(x => x.Tax).Sum();
            }
        }
        public decimal Accomodation_Gross
        {
            get
            {
                return Items.Where(x => x.Type == ItemType.Date).Select(x => x.Gross).Sum();
            }
        }

        public decimal Extra_Net
        {
            get
            {
                return Items.Where(x => x.Type == ItemType.Extra).Select(x => x.Price_Net).Sum();
            }
        }

        public decimal Extra_Commission
        {
            get
            {
                return Items.Where(x => x.Type == ItemType.Extra).Select(x => x.Commission).Sum();
            }
        }

        public decimal Extra_Tax
        {
            get
            {
                return Items.Where(x => x.Type == ItemType.Extra).Select(x => x.Tax).Sum();
            }
        }

        public decimal Extra_Gross
        {
            get
            {
                return Items.Where(x => x.Type == ItemType.Extra).Select(x => x.Gross).Sum();
            }
        }

        public decimal Total_Net
        {
            get
            {
                return Accomodation_Net + Extra_Net;
            }
        }

        public decimal Total_Commission
        {
            get
            {
                return Accomodation_Commission + Extra_Commission;
            }
        }

        public decimal Total_Tax
        {
            get
            {
                return Accomodation_Tax + Extra_Tax;
            }
        }

        //public decimal Total_Gross { get; set; }

        //old
        public decimal Total_Gross
        {
            get
            {
                return Accomodation_Gross + Extra_Gross;
            }
        }

        public async Task<Quote> BuildQuote(Core.Premises.Premise objPremise, Core.Common.Currency objCurrency, DateTime dtArrival, DateTime dtSecondary, int? iNumGuests = null, bool bUpdate = false, Quote? tmpUpdateModel = null)
        {
            var tmpQuote = new Quote();

            if (bUpdate && tmpUpdateModel != null)
            {
                tmpQuote = tmpUpdateModel;
            }
            else
            {
                var rnd = new Random();
                //need an id - mainly for flexi date search so we can identify different quotes for the same premises
                tmpQuote.Id = rnd.Next(1, 9999);
                tmpQuote.Premises = objPremise;
                tmpQuote.Currency = objCurrency;
            }

            tmpQuote.Arrival = dtArrival;
            tmpQuote.Departure = dtSecondary;

            //check if premise has availability for the selected dates
            var lstAvailability = (await Core.Premises.Availability.FindAllBy_PremiseDatesAsync(objPremise.Id, dtArrival, dtSecondary)).OrderBy(r => r.Night).ToList();

            if (lstAvailability.Count > 0)
            {
                if (lstAvailability.All(r => r.State == Core.Enums.Premises_Premise_Availability.Available))
                {
                    tmpQuote.Availability = Core.Enums.Premises_Premise_Availability.Available;
                }
                else if (lstAvailability.All(r => r.State == Core.Enums.Premises_Premise_Availability.AvailableEnquire))
                {
                    tmpQuote.Availability = Core.Enums.Premises_Premise_Availability.AvailableEnquire;
                }
                else if (lstAvailability.All(r => r.State == Core.Enums.Premises_Premise_Availability.Booked))
                {
                    tmpQuote.Availability = Core.Enums.Premises_Premise_Availability.Booked;
                }
                else if (lstAvailability.All(r => r.State == Core.Enums.Premises_Premise_Availability.BookedExt))
                {
                    tmpQuote.Availability = Core.Enums.Premises_Premise_Availability.BookedExt;
                }
                else if (lstAvailability.All(r => r.State == Core.Enums.Premises_Premise_Availability.OnHold))
                {
                    tmpQuote.Availability = Core.Enums.Premises_Premise_Availability.OnHold;
                }
                else if (lstAvailability.All(r => r.State == Core.Enums.Premises_Premise_Availability.Unavailable))
                {
                    tmpQuote.Availability = Core.Enums.Premises_Premise_Availability.Unavailable;
                }
            }

            var lstItems = new List<Item>();
            var lstSeasonIds = new List<int>();
            var dtSelectedDate = dtArrival;
            var totalDay = (int)(dtSecondary - dtSelectedDate).TotalDays;
            //get the daily rate for each day
            while (dtSelectedDate.Date < dtSecondary.Date)
            {
                var tmpDatePrice = new Item();
                var bGetRate = true;
                var objRate = (await Core.Premises.Seasons.Rate.FindAllBy_PremiseAsync(tmpQuote.Premises.Id, dtIncludes: dtSelectedDate.Date, iPartySize: iNumGuests)).FirstOrDefault();

                if (objRate != null && objRate.Loaded)
                {
                    var calculation_Rate = new Core.Models.PriceCalculation(
                        objRate.Price_POA,
                        objRate.Price_EntryMode,
                        objRate.Price / objRate.No_Nights,
                        true,
                        objRate.Commission_AmountType,
                        objRate.Commission_Amount,
                        objRate.Tax_Exempt,
                        objRate.Tax_Value);

                    if (calculation_Rate.Price_Gross > 0)
                    {
                        tmpDatePrice = tmpQuote.AddItem(calculation_Rate, ItemType.Date, "Rate", objRate.Commission_AmountType, objRate.Commission_Amount, dtSelectedDate: dtSelectedDate);

                        bGetRate = false;
                        //tmpQuote.Total_Gross = calculation_Rate.Price_Gross * totalDay;
                    }

                    calculation_Rate = null;

                    if (objRate.Season_Id.HasValue)
                    {
                        var objSeason = await Core.Premises.Seasons.Season.FindAsync(objRate.Season_Id.Value);

                        if (objSeason != null && objSeason.Loaded)
                        {
                            if (bGetRate)
                            {
                                var objSeasonConfig = await Core.Premises.Config.FindBy_SeasonAsync(objSeason.Id);
                                if (objSeasonConfig != null && objSeasonConfig.Loaded)
                                {
                                    if (objSeasonConfig.NightlyPrice.HasValue)
                                    {
                                        var calculation_SeasonConfig = new Core.Models.PriceCalculation(
                                            false,
                                            objSeasonConfig.PriceEntryMode,
                                            objSeasonConfig.NightlyPrice ?? 0,
                                            true,
                                            objSeasonConfig.Commission_AmountType,
                                            objSeasonConfig.Commission_Amount ?? 0,
                                            objSeasonConfig.Tax_Exempt,
                                            objSeasonConfig.Tax_Value ?? 0);

                                        if (calculation_SeasonConfig.Price_Gross > 0)
                                        {
                                            tmpDatePrice = tmpQuote.AddItem(calculation_SeasonConfig, ItemType.Date, "Season Default", objSeasonConfig.Commission_AmountType, objSeasonConfig.Commission_Amount ?? 0, dtSelectedDate: dtSelectedDate);

                                            bGetRate = false;
                                            //tmpQuote.Total_Gross = calculation_SeasonConfig.Price_Gross * totalDay;
                                        }

                                        calculation_SeasonConfig = null;
                                    }
                                }

                                objSeasonConfig = null;
                            }

                            if (!lstSeasonIds.Contains(objSeason.Id))
                            {
                                lstSeasonIds.Add(objSeason.Id);
                            }
                        }
                        objSeason = null;
                    }
                }

                if (bGetRate)
                {
                    var objConfig = await Core.Premises.Config.FindBy_PremiseAsync(tmpQuote.Premises.Id);

                    if (objConfig != null && objConfig.Loaded)
                    {
                        var calculation_Config = new Core.Models.PriceCalculation(
                            false,
                            objConfig.PriceEntryMode,
                            objConfig.NightlyPrice ?? 0,
                            true,
                            objConfig.Commission_AmountType,
                            objConfig.Commission_Amount ?? 0,
                            objConfig.Tax_Exempt,
                            objConfig.Tax_Value ?? 0);

                        if (calculation_Config.Price_Gross > 0)
                        {
                            tmpDatePrice = tmpQuote.AddItem(calculation_Config, ItemType.Date, "Property Default", objConfig.Commission_AmountType, objConfig.Commission_Amount ?? 0, dtSelectedDate: dtSelectedDate);
                            bGetRate = false;
                            //tmpQuote.Total_Gross = calculation_Config.Price_Gross * totalDay;
                        }

                        calculation_Config = null;
                    }

                    objConfig = null;
                }

                objRate = null;

                lstItems.Add(tmpDatePrice);

                dtSelectedDate = dtSelectedDate.AddDays(1);
            }

            if (lstSeasonIds.Count > 0)
            {
                foreach (var iSeasonId in lstSeasonIds)
                {
                    var lstExtra = await Core.Premises.Seasons.Extra.FindAllBy_SeasonAsync(iSeasonId);

                    if (lstExtra.Count > 0)
                    {
                        foreach (var tmpExtra in lstExtra)
                        {
                            var calculation_Extra = new Core.Models.PriceCalculation(
                                false,
                                tmpExtra.Price_EntryMode,
                                tmpExtra.Price,
                                tmpExtra.Commission_SubjectTo,
                                tmpExtra.Commission_AmountType,
                                tmpExtra.Commission_Amount,
                                tmpExtra.Tax_Exempt,
                                tmpExtra.Tax_Value);

                            var objExtra = tmpQuote.AddItem(calculation_Extra, ItemType.Extra, tmpExtra.Name, tmpExtra.Commission_AmountType, tmpExtra.Commission_Amount, iExtra_Id: tmpExtra.Id, strDescription: tmpExtra.Description);
                            //tmpQuote.Total_Gross = calculation_Extra.Price_Gross * totalDay;
                            lstItems.Add(objExtra);
                        }
                    }
                }
            }

            tmpQuote.Items = lstItems;

            tmpQuote.Snippet = await tmpQuote.GenerateSnippet();

            return tmpQuote;
        }


        public Item AddItem(Core.Models.PriceCalculation calculation, ItemType enumType, string strName, Core.Enums.Shared_NumericValueType enumNumericValueType = Core.Enums.Shared_NumericValueType.Unknown, decimal dCommissionAmount = 0, DateTime? dtSelectedDate = null, int? iExtra_Id = null, string strDescription = "")
        {
            var objItem = new Item
            {
                Type = enumType,
                Commission = calculation.Commission ?? 0,
                CommissionType = enumNumericValueType,
                Commission_Amount = dCommissionAmount,
                Gross = calculation.Price_Gross,
                Editable_Gross = calculation.Price_Gross,
                Price_Net = calculation.Price_Net,
                Editable_Price_Net = calculation.Price_Net,
                Tax = calculation.Tax ?? 0,
                Editable_Tax = calculation.Tax ?? 0
            };

            if (enumType == ItemType.Date)
            {
                objItem.Calculated_From = strName;
                objItem.Editable_Commission = calculation.Commission ?? 0;
                objItem.Night = dtSelectedDate ?? DateTime.UtcNow;
            }
            else if (enumType == ItemType.Extra)
            {
                objItem.Name = strName;
                objItem.Description = strDescription;
                objItem.Extra_Id = iExtra_Id ?? 0;
            }

            return objItem;
        }

        public async Task<string> GenerateSnippet()
        {
            var strReturn = "";
            if (Availability == Core.Enums.Premises_Premise_Availability.Available)
            {
                var strLocation = "";
                var strInclusions = "";
                var strDescription = "";
                var strPremises = new StringBuilder();

                var objSeason = await Core.Premises.Seasons.Season.FindBy_PremiseDateAsync(Premises.Id, Arrival);
                if (objSeason != null && objSeason.Loaded)
                {
                    var objSeasonConfig = await Core.Premises.Config.FindBy_SeasonAsync(objSeason.Id);
                    if (objSeasonConfig != null && objSeasonConfig.Loaded)
                    {
                        strDescription = objSeasonConfig.DescriptionForQuote_Calculated;
                        strInclusions = objSeasonConfig.Inclusions_Calculated;
                    }

                    objSeasonConfig = null;
                }

                objSeason = null;

                if (string.IsNullOrWhiteSpace(strInclusions) || string.IsNullOrWhiteSpace(strDescription))
                {
                    var objConfig = await Core.Premises.Config.FindBy_PremiseAsync(Premises.Id);

                    if (objConfig != null && objConfig.Loaded)
                    {
                        if (string.IsNullOrWhiteSpace(strDescription))
                        {
                            strDescription = objConfig.DescriptionForQuote_Calculated;
                        }

                        if (string.IsNullOrWhiteSpace(strInclusions))
                        {
                            strInclusions = objConfig.Inclusions_Calculated;
                        }
                    }

                    objConfig = null;
                }

                if (Premises.Country_Id.HasValue)
                {
                    var objCountry = await Core.Common.Country.FindAsync(Premises.Country_Id.Value);

                    if (objCountry != null && objCountry.Loaded)
                    {
                        strLocation = objCountry.Name;
                    }
                    objCountry = null;
                }

                if (Premises.Region_Id.HasValue)
                {
                    var objRegion = await Core.Common.Region.FindAsync(Premises.Region_Id.Value);

                    if (objRegion != null && objRegion.Loaded)
                    {
                        if (!string.IsNullOrWhiteSpace(strLocation))
                        {
                            strLocation += " - ";
                        }
                        strLocation += objRegion.Name;
                    }

                    objRegion = null;
                }

                //_ = strPremises.AppendFormat("<h3>{0} {1} - {2}</h3>", Premises.Display_Name_Calculated, Arrival.ToString("dd/MM/yyyy"), Departure.ToString("dd/MM/yyyy"));
                _ = strPremises.AppendFormat("<h3><a href='{0}' target='_blank' style='cursor:pointer;'>{1}</a>",Premises.Website_URL,Premises.Display_Name_Calculated);
                if (!string.IsNullOrWhiteSpace(strLocation))
                {
                    _ = strPremises.AppendFormat("<p>{0}</p>", strLocation);
                }

                if (!string.IsNullOrWhiteSpace(strDescription))
                {
                    _ = strPremises.AppendFormat("<p>{0}</p>", strDescription);
                }

                //if (Premises.Guests_Max > 0)
                //{
                //    _ = strPremises.AppendFormat("<p><strong>Guests:</strong> {0}</p>", Premises.Guests_Max);
                //}

                if (Premises.Guests_Max > 0)
                {
                    _ = strPremises.AppendFormat("<p><strong>Sleeps:</strong> {0}</p>", Premises.Guests_Max);
                }
                else
                {
                    _ = strPremises.AppendFormat("<p><strong>Sleeps:</strong> {0}</p>", 0);
                }

                // get bedroom information
                var lstRooms = await Core.Premises.Room.FindAllBy_PremiseAsync(Premises.Id);

                if (lstRooms.Any(r => r.Type == VC.Shared.Enums.Premises_Room_Type.Bedroom))
                {
                    _ = strPremises.AppendFormat("<p><strong>Bedrooms:</strong> {0}</p>", lstRooms.Count(r => r.Type == VC.Shared.Enums.Premises_Room_Type.Bedroom));
                }
                else
                {
                    _ = strPremises.AppendFormat("<p><strong>Bedrooms:</strong> {0}</p>", 0);
                }

                lstRooms = null;

                if (Premises.Rooms_NoBathrooms > 0)
                {
                    _ = strPremises.AppendFormat("<p><strong>Bathrooms:</strong> {0}</p>", Premises.Rooms_NoBathrooms);
                }
                else
                {
                    _ = strPremises.AppendFormat("<p><strong>Bathrooms:</strong> {0}</p>", 0);
                }

                if (Total_Gross > 0)
                {
                    _ = strPremises.AppendFormat("<p><strong>Price includes :</strong> {0}</p>", Core.Common.Currency.Format(Currency, string.Format("{0:n2}", Total_Gross)));
                }
                else
                {
                    _ = strPremises.AppendFormat("<p><strong>Price includes :</strong> {0}</p>", Core.Common.Currency.Format(Currency, string.Format("{0:n2}", 0)));
                }
                // old
                //if (Premises.Size > 0)
                //{
                //    //_ = strPremises.AppendFormat("<p><strong>{0} SQM</strong></p>", Premises.Size.Value);
                //}

                
                //if (!string.IsNullOrWhiteSpace(strInclusions))
                //{
                //    _ = strPremises.AppendFormat("<p><strong>Inclusions:</strong> {0}</p>", strInclusions);
                //}

                //if (!string.IsNullOrWhiteSpace(Premises.Website_URL))
                //{
                //    _ = strPremises.AppendFormat("<p><a href='{0}'>View on website</a></p>", string.Format("{0}/{1}", Core.Settings.Global.Fetch.Website_URL.TrimEnd('/'), Premises.Website_URL.TrimStart('/')));
                //}

                //_ = strPremises.Append("<table>");
                //_ = strPremises.Append("<thead><tr><td><strong>Item</strong></td><td><strong>Description</strong></td><td><strong>Total</strong></td></tr></thead>");
                //_ = strPremises.Append("<tbody>");
                //_ = strPremises.AppendFormat("<tr><td>Accommodation</td><td></td><td>{0}</td></tr>", Core.Common.Currency.Format(Currency, string.Format("{0:n2}", Accomodation_Gross)));

                //foreach (var tmpExtra in Items.Where(x => x.Type == ItemType.Extra))
                //{
                //    _ = strPremises.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", tmpExtra.Name, tmpExtra.Description, Core.Common.Currency.Format(Currency, string.Format("{0:n2}", tmpExtra.Gross)));
                //}

                //_ = strPremises.AppendFormat("<tr><td><strong>Total</strong></td><td></td><td><strong>{0}</strong></td></tr>", Core.Common.Currency.Format(Currency, string.Format("{0:n2}", Total_Gross)));
                //_ = strPremises.Append("</tbody>");
                //_ = strPremises.Append("</table>");

                strReturn = strPremises.ToString();
            }
            else
            {
                strReturn = "";
            }

            return strReturn;
        }
    }

    public class Item
    {
        public ItemType Type { get; set; } = ItemType.Unknown;
        public DateTime Night { get; set; } = DateTime.UtcNow;
        public string Calculated_From { get; set; } = "";
        public decimal Price_Net { get; set; } = 0;
        public decimal Commission { get; set; } = 0;
        public decimal Commission_Amount { get; set; } = 0;
        public Core.Enums.Shared_NumericValueType CommissionType { get; set; } = Core.Enums.Shared_NumericValueType.Unknown;
        public decimal Tax { get; set; } = 0;
        public decimal Gross { get; set; } = 0;
        public decimal Editable_Price_Net { get; set; } = 0;
        public decimal Editable_Commission { get; set; } = 0;
        public decimal Editable_Tax { get; set; } = 0;
        public decimal Editable_Gross { get; set; } = 0;
        public int Extra_Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        //Kept as single function within the item, update is called when updating in the grid and is being passed Item
        public async Task<bool> Update(Quote tmpPremise, ItemType enumType)
        {
            var tmpItem = new Item();

            if (enumType == ItemType.Date)
            {
                tmpItem = tmpPremise.Items.FirstOrDefault(x => x.Type == ItemType.Date && x.Night.Date == Night.Date);
            }
            else if (enumType == ItemType.Extra)
            {
                tmpItem = tmpPremise.Items.FirstOrDefault(x => x.Type == ItemType.Extra && x.Extra_Id == Extra_Id);
            }
            else
            {
                return false;
            }

            if (tmpItem == null)
            {
                return false;
            }

            decimal dEditable_Price = 0;
            decimal dCommission_Amount = 0;
            var bTaxExempt = false;
            var bCommissionSubjectTo = true;
            var enumPriceType = Core.Enums.Shared_PriceValueType.Unknown;
            var enumCommissionType = tmpItem.CommissionType;

            //check if gross has been changed, if so use that to calculate other prices
            if (Editable_Gross != Gross)
            {
                dEditable_Price = Editable_Gross;
                enumPriceType = Core.Enums.Shared_PriceValueType.Gross;
            }
            else if (Editable_Price_Net != Price_Net)
            {
                dEditable_Price = Editable_Price_Net;
                enumPriceType = Core.Enums.Shared_PriceValueType.Net;
            }
            else
            {
                //neiter net or gross updated, so use net price
                dEditable_Price = Editable_Price_Net;
                enumPriceType = Core.Enums.Shared_PriceValueType.Net;
            }

            if (Editable_Tax != Tax)
            {
                // tax amount has changed
                if (Editable_Tax > 0)
                {
                    bTaxExempt = false;
                }
                else
                {
                    bTaxExempt = true;
                }
            }

            if (Editable_Commission != Commission)
            {
                //commission has been manually changed so set price as fixed
                enumCommissionType = Core.Enums.Shared_NumericValueType.Fixed;
                if (Editable_Commission > 0)
                {
                    bCommissionSubjectTo = true;
                }
                else
                {
                    bCommissionSubjectTo = false;
                }

                dCommission_Amount = Editable_Commission;
            }
            else
            {
                if (Editable_Commission > 0)
                {
                    bCommissionSubjectTo = true;
                }

                if (enumCommissionType == Core.Enums.Shared_NumericValueType.Percentage)
                {
                    dCommission_Amount = Commission_Amount;
                }
                else
                {
                    dCommission_Amount = Editable_Commission;
                }
            }


            var calculation_Rate = new Core.Models.PriceCalculation(
                    false,
                    enumPriceType,
                    dEditable_Price,
                    bCommissionSubjectTo,
                    enumCommissionType,
                    dCommission_Amount,
                    bTaxExempt,
                    Editable_Tax);

            tmpItem.Description = Description;
            tmpItem.CommissionType = enumCommissionType;
            tmpItem.Commission = calculation_Rate.Commission ?? 0;
            tmpItem.Editable_Commission = calculation_Rate.Commission ?? 0;
            tmpItem.Gross = calculation_Rate.Price_Gross;
            tmpItem.Editable_Gross = calculation_Rate.Price_Gross;
            tmpItem.Price_Net = calculation_Rate.Price_Net;
            tmpItem.Editable_Price_Net = calculation_Rate.Price_Net;
            tmpItem.Tax = calculation_Rate.Tax ?? 0;
            tmpItem.Editable_Tax = calculation_Rate.Tax ?? 0;
            tmpPremise.Snippet = await tmpPremise.GenerateSnippet();
            return true;
        }
    }
}
