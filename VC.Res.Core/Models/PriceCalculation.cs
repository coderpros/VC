namespace VC.Res.Core.Models
{
    public class PriceCalculation
    {
        public bool POA { get; private set; } = false;

        public Enums.Shared_PriceValueType Price_EntryMode { get; private set; } = Enums.Shared_PriceValueType.Net;
        public decimal Price { get; private set; } = 0;

        public bool Commission_SubjectTo { get; private set; } = false;
        public Enums.Shared_NumericValueType Commission_AmountType { get; private set; } = Enums.Shared_NumericValueType.Percentage;
        public decimal Commission_Amount { get; private set; } = 0;

        public bool Tax_Exempt { get; private set; } = false;
        public decimal Tax_Value { get; private set; } = 0;


        public decimal Price_Net { get; private set; } = 0;

        public decimal? Commission { get; private set; } = null;

        public decimal? Tax { get; private set; } = null;

        public decimal Price_Gross { get; private set; } = 0;


        public PriceCalculation() { }

        public PriceCalculation(bool bPOA, Enums.Shared_PriceValueType enumPriceType, decimal dPrice, bool bSubjectToCommission, Enums.Shared_NumericValueType enumCommissionType, decimal dCommissionValue, bool bTaxExempt, decimal dTaxValue)
        {
            POA = bPOA;
            Price_EntryMode = enumPriceType;
            Price = dPrice;

            Commission_SubjectTo = bSubjectToCommission;
            Commission_AmountType = enumCommissionType;
            Commission_Amount = dCommissionValue;

            Tax_Exempt = bTaxExempt;
            Tax_Value = dTaxValue;

            Calculate();
        }


        private void Calculate()
        {
            try
            {
                if (POA)
                {
                    Price_Net = 0;
                    Commission = null;
                    Tax = null;
                    Price_Gross = 0;

                    return;
                }

                switch (Price_EntryMode)
                {
                    case Enums.Shared_PriceValueType.Net:
                        {
                            // need to calculate towards gross
                            // net + commission + tax = gross
                            Price_Net = Price;

                            // only calculate if the price is subject to commission
                            if (Commission_SubjectTo)
                            {
                                switch (Commission_AmountType)
                                {
                                    case Enums.Shared_NumericValueType.Fixed:
                                        {
                                            // apply the value as the total commission
                                            Commission = Commission_Amount;
                                        }
                                        break;

                                    case Enums.Shared_NumericValueType.Percentage:
                                        {
                                            
                                            if (Price_Net > 0 && Commission_Amount > 0)
                                            {
                                                // old
                                                //Commission = Math.Round(Price_Net / 100 * Commission_Amount, 6, MidpointRounding.AwayFromZero);
                                                // new 
                                                
                                                Commission = Price_Net / (1 - Commission_Amount / 100) - Price_Net;

                                            }
                                            else
                                            {
                                                Commission = 0;
                                            }
                                            //// work out percentage from net price
                                            //if (Price_Net > 0 && Commission_Amount > 0)
                                            //{
                                            //    // old
                                            //    //Commission = Math.Round(Price_Net / 100 * Commission_Amount, 6, MidpointRounding.AwayFromZero);
                                            //    // new 
                                            //    var gross = Price_Net - (Price_Net / (1 - Commission_Amount));
                                            //    Commission = Price_Net - (Price_Net / (1 - Commission_Amount));

                                            //}
                                            //else
                                            //{
                                            //    Commission = 0;
                                            //}
                                        }
                                        break;
                                }
                            }

                            // only apply tax if not exempt
                            if (!Tax_Exempt)
                            {
                                var dAmountToTax = Price_Net + (Commission ?? 0);
                                //new 
                                if (Price_Net > 0 && Tax_Value > 0)
                                {
                                    // old
                                    //Tax = Math.Round(dAmountToTax / 100 * Tax_Value, 6, MidpointRounding.AwayFromZero);
                                    //new 
                                    Tax = dAmountToTax / (1 - Tax_Value / 100) - dAmountToTax;
                                }
                                else
                                {
                                    Tax = 0;
                                }
                                // old
                                //if (dAmountToTax > 0 && Tax_Value > 0)
                                //{
                                //    // old
                                //    //Tax = Math.Round(dAmountToTax / 100 * Tax_Value, 6, MidpointRounding.AwayFromZero);
                                //    //new 
                                //    //Tax = Math.Round(gross - Price_Net, 6, MidpointRounding.AwayFromZero);
                                //}
                                //else
                                //{
                                //    Tax = 0;
                                //}
                            }

                            Price_Gross = Price_Net + (Commission ?? 0) + (Tax ?? 0);
                        }
                        break;

                    case Enums.Shared_PriceValueType.Gross:
                        {
                            // need to calculate backwards to net
                            // gross - tax - commission = net
                            Price_Gross = Price;
                            //var netPrice = Price_Gross * (1 - (Tax_Value / 100) * (1 - (Commission_Amount / 100)));
                            //var priceAfterCommision = Price_Gross * (1 - (Commission_Amount / 100));
                            if (!Tax_Exempt)
                            {
                                if (Price_Gross > 0 && Tax_Value > 0)
                                {
                                    // old
                                    //Tax = Math.Round(Price_Gross / (100 + Tax_Value) * Tax_Value, 6, MidpointRounding.AwayFromZero); 
                                    //new 
                                    Tax = Math.Round(Price_Gross * Tax_Value / 100, 6, MidpointRounding.AwayFromZero);

                                    //Tax = Price_Gross - (Price_Gross * (1 - (Tax_Value / 100)));
                                }
                                else
                                {
                                    Tax = 0;
                                }
                            }

                            if (Commission_SubjectTo)
                            {
                                switch (Commission_AmountType)
                                {
                                    case Enums.Shared_NumericValueType.Fixed:
                                        {
                                            // apply the value as the total commission
                                            Commission =  Commission_Amount;
                                        }
                                        break;

                                    case Enums.Shared_NumericValueType.Percentage:
                                        {
                                            var dAmountForCommission = Price_Gross - (Tax ?? 0);

                                            // work out percentage from net price
                                            if (dAmountForCommission > 0 && Commission_Amount > 0)
                                            {
                                                // old
                                                //Commission = Math.Round(dAmountForCommission / (100 + Commission_Amount) * Commission_Amount, 6, MidpointRounding.AwayFromZero);
                                                // new 
                                                Commission = Math.Round(dAmountForCommission * Commission_Amount / 100, 6, MidpointRounding.AwayFromZero);
                                                //Commission = Price_Gross - (Price_Gross * (1 - (Commission_Amount / 100)));
                                            }
                                            else
                                            {
                                                Commission = 0;
                                            }
                                        }
                                        break;
                                }
                            }

                            //Price_Net = netPrice;

                            Price_Net = Price_Gross - (Tax ?? 0) - (Commission ?? 0);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(PriceCalculation).ToString(), "Calculate()", ex);
            }
        }
    }
}
