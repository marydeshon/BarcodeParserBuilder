﻿using System.Collections.ObjectModel;
using BarcodeParserBuilder.Barcodes.CODE39;
using BarcodeParserBuilder.Infrastructure;

namespace BarcodeParserBuilder.Barcodes
{
    public abstract class Barcode
    {
        
        public Barcode() { }

        /// <summary>
        /// If the reader returns the reader modifier, then it can be passed to barcode.
        /// But the barcode should still be usable without the reader modifier information
        /// </summary>
        /// <param name="modifier">Parsed and validated modifier</param>
        public Barcode(AimReaderModifier modifier)
        {
            ReaderModifier = modifier;
        }

        protected abstract FieldCollection BarcodeFields { get; }

        // Public Barcode Properties 
        /// <summary>
        /// Read only list of the barcode's internal fields
        /// </summary>
        public ReadOnlyFieldCollection Fields
        {
            get
            {
                _fields ??= new ReadOnlyFieldCollection(BarcodeFields);
                return _fields;
            }
        }
        private ReadOnlyFieldCollection? _fields = null;
        public abstract BarcodeType BarcodeType { get; }
        public abstract ProductCode? ProductCode { get; set; }
        public abstract BarcodeDateTime? ExpirationDate { get; set; }
        public abstract BarcodeDateTime? ProductionDate { get; set; }
        public abstract string? BatchNumber { get; set; }
        public abstract string? SerialNumber { get; set; }

        /// <summary>
        /// ReaderModifier is not part of the barcode but is crucial to interpret the reading correctly.
        /// Barcode readers can be configured to behave differently on the same reading and reader modifier informs about it
        /// </summary>
        public AimReaderModifier? ReaderModifier { get; private set; }

    }

    public class FieldCollection : KeyedCollection<string, IBarcodeField>
    {
        protected override string GetKeyForItem(IBarcodeField item) => item.Identifier;
    }

    public class ReadOnlyFieldCollection : ReadOnlyCollection<IBarcodeField>
    {
        private readonly KeyedCollection<string, IBarcodeField> _innerCollection;

        public ReadOnlyFieldCollection(KeyedCollection<string, IBarcodeField> collection) : base(collection)
        {
            _innerCollection = collection ?? throw new ArgumentException($"{nameof(collection)} should not be null");
        }

        public IBarcodeField this[string identifier] => _innerCollection[identifier];

        internal bool Contains(string identifier) => _innerCollection.Contains(identifier);
    }

    
}
