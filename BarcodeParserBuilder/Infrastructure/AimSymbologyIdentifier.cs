﻿using System.Reflection;

namespace BarcodeParserBuilder.Infrastructure
{
    /// <summary>
    /// AIM identifier (now actually ISO/IEC 15424:2008(E) standard) specifies a preamble message generated by the reader
    /// and interpretable by the receiving system.  It indicates the bar code symbology or other origin of transmitted data,
    /// together with details of certain specified optional processing features associated with the data message.
    /// 
    /// The AIM identifer is 3 character preamble in the reading, where ] is the "flag character"
    /// and indicates to the host that it and the characters following are the symbology identifier characters.
    /// 
    /// Code character is second character in the symbology identifier string,
    /// which usually indicates to the host the bar code symbology of the symbol which has been read.
    /// Modifier character(s) are one or more characters following the code character in the symbology identifier string,
    /// indicating optional features or processing applied to the symbol.
    ///
    /// The precise interpretation of the modifier character should be obtained by reference to the relevant symbology specification.
    /// The modifier characters define the options available for the code character.
    /// The number of modifier characters and their meaning is defined for each of the code characters.
    /// The first modifier character shall be from the set { 0 to 9, A to Z, a to z};
    /// in some instances the character may represent a hexadecimal value(0 to F) corresponding to the sum of active processing options.
    ///
    /// How it is used: successfully parsed Barcode object may (when particular parser implements it) have set the property ReaderInformation of type AimSymbologyIdentifier
    /// When present, it should provide to the user the information how particular reader processed the barcode.
    /// The processing depends from the physical barcode printout and reader settings
    /// 
    /// How to implement for particular barcode type:
    /// Minimum:
    /// Implement the subclass of AimSymbologyIdentifier and in the particular ParserBuilder use the factory method to initialize
    /// the AimSymbologyIdentifier of particular type:
    /// for example Code39SymbologyIdentifier code39identifier = AimSymbologyIdentifier.FromRawReading<Code39SymbologyIdentifier>(inputBarcode!);
    /// and then use it to initialize the barcode:
    /// new Code39Barcode(code39identifier)
    /// Recommended:
    /// Implement also static properties which names and values represent all possible and allowed types of AIM identifier values
    /// </summary>
    public abstract class AimSymbologyIdentifier : IComparable
    {
        public static readonly string AIMSYMBOLOGYFLAG = "]";

        public string SymbologyIdentifier { get; private set; }
        public string CodeCharacter { get => SymbologyIdentifier[0].ToString(); }

        public string ModifierCharacter { get => SymbologyIdentifier[1].ToString(); }

        public AimSymbologyIdentifier() { }

        public AimSymbologyIdentifier(string symbologyIdentifier)
        {
            SymbologyIdentifier = symbologyIdentifier;
        }

        public static TAimIdentifier FromRawReading<TAimIdentifier>(string rawReading) where TAimIdentifier : AimSymbologyIdentifier, new()
        {
            if (rawReading == null)
            {
                throw new ArgumentNullException(nameof(rawReading));
            }

            if (!rawReading.StartsWith(AIMSYMBOLOGYFLAG))
            {
                throw new ArgumentException("Reading does not start with AIM symbology flag");
            }

            if (rawReading.Length < 3)
            {
                throw new ArgumentException("Reading is too short");
            }

            var symbologyIdentifierImpl = new TAimIdentifier();
            symbologyIdentifierImpl.ParseReading(rawReading);
            return symbologyIdentifierImpl;
        }

        public static string StripSymbologyIdentifier(string barcodeString)
        {
            if (barcodeString == null)
            {
                throw new ArgumentNullException(nameof(barcodeString));
            }

            if (!barcodeString.StartsWith(AIMSYMBOLOGYFLAG) || barcodeString.Length <= 3)
                return barcodeString;

            return barcodeString[3..];
        }

        protected void ParseReading(string rawReading)
        {
            SymbologyIdentifier = rawReading!.StartsWith(AIMSYMBOLOGYFLAG)
                ? rawReading[1..3].ToString()
                : rawReading[0..2].ToString();

        }

        public override string ToString() => SymbologyIdentifier;

        public static IEnumerable<T> GetAll<T>() where T : AimSymbologyIdentifier =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<T>();

        public static IEnumerable<string> GetAllValues<T>() where T : AimSymbologyIdentifier =>
            typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f => f.GetValue(f).ToString());


        public override bool Equals(object obj)
        {
            return obj is AimSymbologyIdentifier aimModifier && aimModifier.SymbologyIdentifier == this.SymbologyIdentifier;
        }

        public int CompareTo(object other) => SymbologyIdentifier.CompareTo(((AimSymbologyIdentifier)other).SymbologyIdentifier);

        public override int GetHashCode()
        {
            return SymbologyIdentifier.GetHashCode();
        }
    }
}
