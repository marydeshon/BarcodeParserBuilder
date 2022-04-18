﻿using BarcodeParserBuilder.Barcodes;

namespace BarcodeParserBuilder
{
    public interface IBarcodeParserBuilder
    {
        public string Build(Barcode barcode);
        public bool TryParse(string barcodeString, out Barcode barcode);
        public bool TryParse(string barcodeString, out Barcode barcode, out string feedback);
    }
}