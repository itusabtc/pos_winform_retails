namespace NailsChekin.Models.Reports
{
    partial class CloseOutReceipt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.detailTable = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow4 = new DevExpress.XtraReports.UI.XRTableRow();
            this.date = new DevExpress.XtraReports.UI.XRTableCell();
            this.discount = new DevExpress.XtraReports.UI.XRTableCell();
            this.supply = new DevExpress.XtraReports.UI.XRTableCell();
            this.tip = new DevExpress.XtraReports.UI.XRTableCell();
            this.sale = new DevExpress.XtraReports.UI.XRTableCell();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.GroupHeader2 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.customerTable = new DevExpress.XtraReports.UI.XRTable();
            this.customerNameRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.customerLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.dateRange = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow3 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow6 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTotal_Revenue = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTotal_Revenue_Value = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow5 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTotal_Info = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTotal_Info_Value = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow9 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell11 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow7 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTotal_Customer_Value = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow8 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrCustomer_Info = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrCustomer_Info_Value = new DevExpress.XtraReports.UI.XRTableCell();
            this.customerAddressRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.customerTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.vendorTable = new DevExpress.XtraReports.UI.XRTable();
            this.vendorNameRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.vendorName = new DevExpress.XtraReports.UI.XRTableCell();
            this.vendorAddressRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.vendorAddress = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.vendorFullAddress = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.vendorPhone = new DevExpress.XtraReports.UI.XRTableCell();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.headerTable = new DevExpress.XtraReports.UI.XRTable();
            this.headerTableRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.productNameCaption = new DevExpress.XtraReports.UI.XRTableCell();
            this.discountCaption = new DevExpress.XtraReports.UI.XRTableCell();
            this.supplyDeductCaption = new DevExpress.XtraReports.UI.XRTableCell();
            this.lineTotalCaption = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.baseControlStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            this.simpleTextStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            this.captionsStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customerTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vendorTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.detailTable});
            this.Detail.HeightF = 46F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StyleName = "baseControlStyle";
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // detailTable
            // 
            this.detailTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.detailTable.Name = "detailTable";
            this.detailTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 96F);
            this.detailTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow4});
            this.detailTable.SizeF = new System.Drawing.SizeF(285F, 25F);
            // 
            // xrTableRow4
            // 
            this.xrTableRow4.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.date,
            this.discount,
            this.supply,
            this.tip,
            this.sale});
            this.xrTableRow4.Name = "xrTableRow4";
            this.xrTableRow4.Weight = 1D;
            // 
            // date
            // 
            this.date.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.date.Multiline = true;
            this.date.Name = "date";
            this.date.StylePriority.UseFont = false;
            this.date.StylePriority.UseTextAlignment = false;
            this.date.Text = "Nails";
            this.date.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.date.Weight = 1.4912280701754386D;
            // 
            // discount
            // 
            this.discount.Multiline = true;
            this.discount.Name = "discount";
            this.discount.StylePriority.UseTextAlignment = false;
            this.discount.Text = "$0.00";
            this.discount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.discount.Weight = 0.87719298245614041D;
            // 
            // supply
            // 
            this.supply.Multiline = true;
            this.supply.Name = "supply";
            this.supply.StylePriority.UseTextAlignment = false;
            this.supply.Text = "$0.00";
            this.supply.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.supply.Weight = 0.8771929824561403D;
            // 
            // tip
            // 
            this.tip.Multiline = true;
            this.tip.Name = "tip";
            this.tip.StylePriority.UseTextAlignment = false;
            this.tip.Text = "$0.00";
            this.tip.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tip.Weight = 0.87719298245614041D;
            // 
            // sale
            // 
            this.sale.Multiline = true;
            this.sale.Name = "sale";
            this.sale.StylePriority.UseTextAlignment = false;
            this.sale.Text = "$0.00";
            this.sale.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.sale.Weight = 0.87719298245614041D;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 0F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.StylePriority.UseBackColor = false;
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 0F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // GroupHeader2
            // 
            this.GroupHeader2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine1,
            this.customerTable,
            this.vendorTable});
            this.GroupHeader2.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("InvoiceNumber", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeader2.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
            this.GroupHeader2.HeightF = 323.5417F;
            this.GroupHeader2.Level = 1;
            this.GroupHeader2.Name = "GroupHeader2";
            this.GroupHeader2.StyleName = "baseControlStyle";
            this.GroupHeader2.StylePriority.UseBackColor = false;
            // 
            // xrLine1
            // 
            this.xrLine1.ForeColor = System.Drawing.Color.Silver;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 313.5417F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(295F, 10.00002F);
            this.xrLine1.StylePriority.UseForeColor = false;
            // 
            // customerTable
            // 
            this.customerTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 109.9999F);
            this.customerTable.Name = "customerTable";
            this.customerTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.customerNameRow,
            this.xrTableRow3,
            this.xrTableRow6,
            this.xrTableRow5,
            this.xrTableRow9,
            this.xrTableRow7,
            this.xrTableRow8,
            this.customerAddressRow});
            this.customerTable.SizeF = new System.Drawing.SizeF(285F, 200.0001F);
            this.customerTable.StyleName = "simpleTextStyle";
            // 
            // customerNameRow
            // 
            this.customerNameRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.customerLabel,
            this.dateRange});
            this.customerNameRow.Name = "customerNameRow";
            this.customerNameRow.Weight = 1D;
            // 
            // customerLabel
            // 
            this.customerLabel.CanShrink = true;
            this.customerLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.customerLabel.ForeColor = System.Drawing.Color.DimGray;
            this.customerLabel.Name = "customerLabel";
            this.customerLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.customerLabel.StyleName = "captionsStyle";
            this.customerLabel.StylePriority.UseFont = false;
            this.customerLabel.StylePriority.UseForeColor = false;
            this.customerLabel.StylePriority.UsePadding = false;
            this.customerLabel.StylePriority.UseTextAlignment = false;
            this.customerLabel.Text = "Date:";
            this.customerLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.customerLabel.Weight = 0.33383248329962151D;
            // 
            // dateRange
            // 
            this.dateRange.CanShrink = true;
            this.dateRange.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateRange.Name = "dateRange";
            this.dateRange.StylePriority.UseFont = false;
            this.dateRange.StylePriority.UsePadding = false;
            this.dateRange.Text = "09/01/2022";
            this.dateRange.Weight = 1.5332903003399065D;
            // 
            // xrTableRow3
            // 
            this.xrTableRow3.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1});
            this.xrTableRow3.Name = "xrTableRow3";
            this.xrTableRow3.Weight = 1D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell1.ForeColor = System.Drawing.Color.DimGray;
            this.xrTableCell1.Multiline = true;
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrTableCell1.StyleName = "captionsStyle";
            this.xrTableCell1.StylePriority.UseFont = false;
            this.xrTableCell1.StylePriority.UseForeColor = false;
            this.xrTableCell1.StylePriority.UsePadding = false;
            this.xrTableCell1.StylePriority.UseTextAlignment = false;
            this.xrTableCell1.Text = "=============================";
            this.xrTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell1.Weight = 1.8671227836395281D;
            // 
            // xrTableRow6
            // 
            this.xrTableRow6.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTotal_Revenue,
            this.xrTotal_Revenue_Value});
            this.xrTableRow6.Name = "xrTableRow6";
            this.xrTableRow6.Weight = 1D;
            // 
            // xrTotal_Revenue
            // 
            this.xrTotal_Revenue.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTotal_Revenue.ForeColor = System.Drawing.Color.DimGray;
            this.xrTotal_Revenue.Multiline = true;
            this.xrTotal_Revenue.Name = "xrTotal_Revenue";
            this.xrTotal_Revenue.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrTotal_Revenue.StyleName = "captionsStyle";
            this.xrTotal_Revenue.StylePriority.UseFont = false;
            this.xrTotal_Revenue.StylePriority.UseForeColor = false;
            this.xrTotal_Revenue.StylePriority.UsePadding = false;
            this.xrTotal_Revenue.StylePriority.UseTextAlignment = false;
            this.xrTotal_Revenue.Text = "TOTAL REVENUE";
            this.xrTotal_Revenue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTotal_Revenue.Weight = 1.3097042727484909D;
            // 
            // xrTotal_Revenue_Value
            // 
            this.xrTotal_Revenue_Value.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTotal_Revenue_Value.Multiline = true;
            this.xrTotal_Revenue_Value.Name = "xrTotal_Revenue_Value";
            this.xrTotal_Revenue_Value.StylePriority.UseFont = false;
            this.xrTotal_Revenue_Value.StylePriority.UsePadding = false;
            this.xrTotal_Revenue_Value.StylePriority.UseTextAlignment = false;
            this.xrTotal_Revenue_Value.Text = "$0.00";
            this.xrTotal_Revenue_Value.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTotal_Revenue_Value.Weight = 0.55741851089103722D;
            // 
            // xrTableRow5
            // 
            this.xrTableRow5.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTotal_Info,
            this.xrTotal_Info_Value});
            this.xrTableRow5.Name = "xrTableRow5";
            this.xrTableRow5.Weight = 1D;
            // 
            // xrTotal_Info
            // 
            this.xrTotal_Info.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTotal_Info.ForeColor = System.Drawing.Color.DimGray;
            this.xrTotal_Info.Multiline = true;
            this.xrTotal_Info.Name = "xrTotal_Info";
            this.xrTotal_Info.Padding = new DevExpress.XtraPrinting.PaddingInfo(15, 0, 0, 0, 100F);
            this.xrTotal_Info.StyleName = "captionsStyle";
            this.xrTotal_Info.StylePriority.UseFont = false;
            this.xrTotal_Info.StylePriority.UseForeColor = false;
            this.xrTotal_Info.StylePriority.UsePadding = false;
            this.xrTotal_Info.StylePriority.UseTextAlignment = false;
            this.xrTotal_Info.Text = "CASH";
            this.xrTotal_Info.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTotal_Info.Weight = 1.3097042727484909D;
            // 
            // xrTotal_Info_Value
            // 
            this.xrTotal_Info_Value.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTotal_Info_Value.Multiline = true;
            this.xrTotal_Info_Value.Name = "xrTotal_Info_Value";
            this.xrTotal_Info_Value.StylePriority.UseFont = false;
            this.xrTotal_Info_Value.StylePriority.UsePadding = false;
            this.xrTotal_Info_Value.StylePriority.UseTextAlignment = false;
            this.xrTotal_Info_Value.Text = "$0.00";
            this.xrTotal_Info_Value.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTotal_Info_Value.Weight = 0.55741851089103722D;
            // 
            // xrTableRow9
            // 
            this.xrTableRow9.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell11});
            this.xrTableRow9.Name = "xrTableRow9";
            this.xrTableRow9.Weight = 1D;
            // 
            // xrTableCell11
            // 
            this.xrTableCell11.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell11.ForeColor = System.Drawing.Color.DimGray;
            this.xrTableCell11.Multiline = true;
            this.xrTableCell11.Name = "xrTableCell11";
            this.xrTableCell11.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrTableCell11.StyleName = "captionsStyle";
            this.xrTableCell11.StylePriority.UseFont = false;
            this.xrTableCell11.StylePriority.UseForeColor = false;
            this.xrTableCell11.StylePriority.UsePadding = false;
            this.xrTableCell11.StylePriority.UseTextAlignment = false;
            this.xrTableCell11.Text = "==============================";
            this.xrTableCell11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell11.Weight = 1.8671227836395281D;
            // 
            // xrTableRow7
            // 
            this.xrTableRow7.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell2,
            this.xrTotal_Customer_Value});
            this.xrTableRow7.Name = "xrTableRow7";
            this.xrTableRow7.Weight = 1D;
            // 
            // xrTableCell2
            // 
            this.xrTableCell2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell2.ForeColor = System.Drawing.Color.DimGray;
            this.xrTableCell2.Multiline = true;
            this.xrTableCell2.Name = "xrTableCell2";
            this.xrTableCell2.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrTableCell2.StyleName = "captionsStyle";
            this.xrTableCell2.StylePriority.UseFont = false;
            this.xrTableCell2.StylePriority.UseForeColor = false;
            this.xrTableCell2.StylePriority.UsePadding = false;
            this.xrTableCell2.StylePriority.UseTextAlignment = false;
            this.xrTableCell2.Text = "TOTAL CUSTOMER";
            this.xrTableCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrTableCell2.Weight = 1.3097042727484909D;
            // 
            // xrTotal_Customer_Value
            // 
            this.xrTotal_Customer_Value.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTotal_Customer_Value.Multiline = true;
            this.xrTotal_Customer_Value.Name = "xrTotal_Customer_Value";
            this.xrTotal_Customer_Value.StylePriority.UseFont = false;
            this.xrTotal_Customer_Value.StylePriority.UsePadding = false;
            this.xrTotal_Customer_Value.StylePriority.UseTextAlignment = false;
            this.xrTotal_Customer_Value.Text = "10";
            this.xrTotal_Customer_Value.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTotal_Customer_Value.Weight = 0.55741851089103722D;
            // 
            // xrTableRow8
            // 
            this.xrTableRow8.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrCustomer_Info,
            this.xrCustomer_Info_Value});
            this.xrTableRow8.Name = "xrTableRow8";
            this.xrTableRow8.Weight = 1D;
            // 
            // xrCustomer_Info
            // 
            this.xrCustomer_Info.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrCustomer_Info.ForeColor = System.Drawing.Color.DimGray;
            this.xrCustomer_Info.Multiline = true;
            this.xrCustomer_Info.Name = "xrCustomer_Info";
            this.xrCustomer_Info.Padding = new DevExpress.XtraPrinting.PaddingInfo(15, 0, 0, 0, 100F);
            this.xrCustomer_Info.StyleName = "captionsStyle";
            this.xrCustomer_Info.StylePriority.UseFont = false;
            this.xrCustomer_Info.StylePriority.UseForeColor = false;
            this.xrCustomer_Info.StylePriority.UsePadding = false;
            this.xrCustomer_Info.StylePriority.UseTextAlignment = false;
            this.xrCustomer_Info.Text = "Frequent Customer";
            this.xrCustomer_Info.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.xrCustomer_Info.Weight = 1.3097042727484909D;
            // 
            // xrCustomer_Info_Value
            // 
            this.xrCustomer_Info_Value.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrCustomer_Info_Value.Multiline = true;
            this.xrCustomer_Info_Value.Name = "xrCustomer_Info_Value";
            this.xrCustomer_Info_Value.StylePriority.UseFont = false;
            this.xrCustomer_Info_Value.StylePriority.UsePadding = false;
            this.xrCustomer_Info_Value.StylePriority.UseTextAlignment = false;
            this.xrCustomer_Info_Value.Text = "0";
            this.xrCustomer_Info_Value.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrCustomer_Info_Value.Weight = 0.55741851089103722D;
            // 
            // customerAddressRow
            // 
            this.customerAddressRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.customerTableCell1});
            this.customerAddressRow.Name = "customerAddressRow";
            this.customerAddressRow.Weight = 1D;
            // 
            // customerTableCell1
            // 
            this.customerTableCell1.CanShrink = true;
            this.customerTableCell1.Name = "customerTableCell1";
            this.customerTableCell1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.customerTableCell1.StylePriority.UseFont = false;
            this.customerTableCell1.StylePriority.UsePadding = false;
            this.customerTableCell1.StylePriority.UseTextAlignment = false;
            this.customerTableCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.customerTableCell1.Weight = 1.8671227836395281D;
            // 
            // vendorTable
            // 
            this.vendorTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10.00001F);
            this.vendorTable.Name = "vendorTable";
            this.vendorTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.vendorNameRow,
            this.vendorAddressRow,
            this.xrTableRow1,
            this.xrTableRow2});
            this.vendorTable.SizeF = new System.Drawing.SizeF(285F, 99.99994F);
            this.vendorTable.StyleName = "simpleTextStyle";
            // 
            // vendorNameRow
            // 
            this.vendorNameRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.vendorName});
            this.vendorNameRow.Name = "vendorNameRow";
            this.vendorNameRow.Weight = 1D;
            // 
            // vendorName
            // 
            this.vendorName.CanShrink = true;
            this.vendorName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vendorName.Name = "vendorName";
            this.vendorName.StylePriority.UseFont = false;
            this.vendorName.StylePriority.UsePadding = false;
            this.vendorName.StylePriority.UseTextAlignment = false;
            this.vendorName.Text = "VendorName";
            this.vendorName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.vendorName.Weight = 1D;
            // 
            // vendorAddressRow
            // 
            this.vendorAddressRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.vendorAddress});
            this.vendorAddressRow.Name = "vendorAddressRow";
            this.vendorAddressRow.Weight = 1D;
            // 
            // vendorAddress
            // 
            this.vendorAddress.CanShrink = true;
            this.vendorAddress.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vendorAddress.Name = "vendorAddress";
            this.vendorAddress.StylePriority.UseFont = false;
            this.vendorAddress.StylePriority.UsePadding = false;
            this.vendorAddress.StylePriority.UseTextAlignment = false;
            this.vendorAddress.Text = "123";
            this.vendorAddress.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.vendorAddress.Weight = 1D;
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.vendorFullAddress});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Weight = 1D;
            // 
            // vendorFullAddress
            // 
            this.vendorFullAddress.Multiline = true;
            this.vendorFullAddress.Name = "vendorFullAddress";
            this.vendorFullAddress.StylePriority.UseFont = false;
            this.vendorFullAddress.StylePriority.UsePadding = false;
            this.vendorFullAddress.StylePriority.UseTextAlignment = false;
            this.vendorFullAddress.Text = "123, Texas 123 ";
            this.vendorFullAddress.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.vendorFullAddress.Weight = 1D;
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.vendorPhone});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.Weight = 1D;
            // 
            // vendorPhone
            // 
            this.vendorPhone.Multiline = true;
            this.vendorPhone.Name = "vendorPhone";
            this.vendorPhone.StylePriority.UseFont = false;
            this.vendorPhone.StylePriority.UsePadding = false;
            this.vendorPhone.StylePriority.UseTextAlignment = false;
            this.vendorPhone.Text = "7039574599";
            this.vendorPhone.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.vendorPhone.Weight = 1D;
            // 
            // GroupHeader1
            // 
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.headerTable});
            this.GroupHeader1.HeightF = 40F;
            this.GroupHeader1.Name = "GroupHeader1";
            this.GroupHeader1.RepeatEveryPage = true;
            this.GroupHeader1.StyleName = "baseControlStyle";
            // 
            // headerTable
            // 
            this.headerTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.headerTable.Name = "headerTable";
            this.headerTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 15, 0, 100F);
            this.headerTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.headerTableRow});
            this.headerTable.SizeF = new System.Drawing.SizeF(285F, 40F);
            this.headerTable.StyleName = "captionsStyle";
            this.headerTable.StylePriority.UsePadding = false;
            // 
            // headerTableRow
            // 
            this.headerTableRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.productNameCaption,
            this.discountCaption,
            this.supplyDeductCaption,
            this.lineTotalCaption,
            this.xrTableCell3});
            this.headerTableRow.Name = "headerTableRow";
            this.headerTableRow.Weight = 11.5D;
            // 
            // productNameCaption
            // 
            this.productNameCaption.Name = "productNameCaption";
            this.productNameCaption.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 10, 15, 0, 100F);
            this.productNameCaption.StylePriority.UsePadding = false;
            this.productNameCaption.Text = "Nails";
            this.productNameCaption.Weight = 0.72270951745387468D;
            // 
            // discountCaption
            // 
            this.discountCaption.Name = "discountCaption";
            this.discountCaption.StylePriority.UsePadding = false;
            this.discountCaption.StylePriority.UseTextAlignment = false;
            this.discountCaption.Text = "DR.";
            this.discountCaption.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.discountCaption.Weight = 0.42512326171096126D;
            // 
            // supplyDeductCaption
            // 
            this.supplyDeductCaption.Name = "supplyDeductCaption";
            this.supplyDeductCaption.StylePriority.UseTextAlignment = false;
            this.supplyDeductCaption.Text = "S.D";
            this.supplyDeductCaption.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.supplyDeductCaption.Weight = 0.42512326848184046D;
            // 
            // lineTotalCaption
            // 
            this.lineTotalCaption.Name = "lineTotalCaption";
            this.lineTotalCaption.StylePriority.UseTextAlignment = false;
            this.lineTotalCaption.Text = "R.";
            this.lineTotalCaption.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.lineTotalCaption.Weight = 0.42512325676281121D;
            // 
            // xrTableCell3
            // 
            this.xrTableCell3.Multiline = true;
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.StylePriority.UseTextAlignment = false;
            this.xrTableCell3.Text = "CCT.";
            this.xrTableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.xrTableCell3.Weight = 0.42512328658147025D;
            // 
            // baseControlStyle
            // 
            this.baseControlStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.baseControlStyle.Name = "baseControlStyle";
            this.baseControlStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            // 
            // simpleTextStyle
            // 
            this.simpleTextStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.simpleTextStyle.ForeColor = System.Drawing.Color.DimGray;
            this.simpleTextStyle.Name = "simpleTextStyle";
            this.simpleTextStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            // 
            // captionsStyle
            // 
            this.captionsStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.captionsStyle.ForeColor = System.Drawing.Color.Black;
            this.captionsStyle.Name = "captionsStyle";
            this.captionsStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            // 
            // CloseOutReceipt
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.GroupHeader2,
            this.GroupHeader1});
            this.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
            this.PageWidth = 315;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.RollPaper = true;
            this.ShowPrintMarginsWarning = false;
            this.ShowPrintStatusDialog = false;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.baseControlStyle,
            this.simpleTextStyle,
            this.captionsStyle});
            this.StyleSheetPath = "";
            this.Version = "19.2";
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customerTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vendorTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader2;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
        private DevExpress.XtraReports.UI.XRControlStyle baseControlStyle;
        private DevExpress.XtraReports.UI.XRControlStyle simpleTextStyle;
        private DevExpress.XtraReports.UI.XRControlStyle captionsStyle;
        private DevExpress.XtraReports.UI.XRTable vendorTable;
        private DevExpress.XtraReports.UI.XRTableRow vendorNameRow;
        private DevExpress.XtraReports.UI.XRTableCell vendorName;
        private DevExpress.XtraReports.UI.XRTableRow vendorAddressRow;
        private DevExpress.XtraReports.UI.XRTableCell vendorAddress;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
        private DevExpress.XtraReports.UI.XRTableCell vendorFullAddress;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
        private DevExpress.XtraReports.UI.XRTableCell vendorPhone;
        private DevExpress.XtraReports.UI.XRTable customerTable;
        private DevExpress.XtraReports.UI.XRTableRow customerNameRow;
        private DevExpress.XtraReports.UI.XRTableCell customerLabel;
        private DevExpress.XtraReports.UI.XRTableCell dateRange;
        private DevExpress.XtraReports.UI.XRTableRow customerAddressRow;
        private DevExpress.XtraReports.UI.XRTableCell customerTableCell1;
        private DevExpress.XtraReports.UI.XRTable headerTable;
        private DevExpress.XtraReports.UI.XRTableRow headerTableRow;
        private DevExpress.XtraReports.UI.XRTableCell productNameCaption;
        private DevExpress.XtraReports.UI.XRTableCell discountCaption;
        private DevExpress.XtraReports.UI.XRTableCell supplyDeductCaption;
        private DevExpress.XtraReports.UI.XRTableCell lineTotalCaption;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell3;
        private DevExpress.XtraReports.UI.XRTable detailTable;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow4;
        private DevExpress.XtraReports.UI.XRTableCell date;
        private DevExpress.XtraReports.UI.XRTableCell discount;
        private DevExpress.XtraReports.UI.XRTableCell supply;
        private DevExpress.XtraReports.UI.XRTableCell tip;
        private DevExpress.XtraReports.UI.XRTableCell sale;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow3;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow6;
        private DevExpress.XtraReports.UI.XRTableCell xrTotal_Revenue;
        private DevExpress.XtraReports.UI.XRTableCell xrTotal_Revenue_Value;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow5;
        private DevExpress.XtraReports.UI.XRTableCell xrTotal_Info;
        private DevExpress.XtraReports.UI.XRTableCell xrTotal_Info_Value;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow7;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell2;
        private DevExpress.XtraReports.UI.XRTableCell xrTotal_Customer_Value;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow8;
        private DevExpress.XtraReports.UI.XRTableCell xrCustomer_Info;
        private DevExpress.XtraReports.UI.XRTableCell xrCustomer_Info_Value;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow9;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell11;
    }
}
