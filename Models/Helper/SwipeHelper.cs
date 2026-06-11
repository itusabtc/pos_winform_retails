using System;
using System.Text.RegularExpressions;

namespace NailsChekin.Models.Helper
{
    public enum SwipeFormat
    {
        Unknown = 0,
        BankTrack1 = 1,
        BankTrack2 = 2,
        GiftV1 = 3,
        GenericTracks = 4,
        PanOnlyTrack2 = 5,
        PostQuestionDigits = 6,
        DigitsOnly = 7
    }

    public sealed class SwipeParseResult
    {
        public bool Ok { get; set; }
        public SwipeFormat Format { get; set; }
        public string CardType { get; set; }      // "BANK", "GIFT", "UNKNOWN"
        public int FormatVersion { get; set; }    // ví dụ: 1 cho GIFT-1
        public string CardNumber { get; set; }    // PAN
        public string RawTrack1 { get; set; }
        public string RawTrack2 { get; set; }
        public string Error { get; set; }
    }

    public static class SwipeHelper
    {
        // Regex compile sẵn
        static readonly Regex RxGift = new Regex(@"^\s*%GIFT-(?<ver>\d+)\?\s*;(?<pan>\d{4,30})\?\s*$");

        // ISO/IEC 7813 – Track 1: %B{PAN}^{NAME}^{DATA}?
        static readonly Regex RxBankT1 = new Regex(@"%B(?<pan>\d{4,19})\^(?<name>[^\\^]{2,26})\^(?<rest>[^?]*)\?");

        // ISO/IEC 7813 – Track 2: ;{PAN}={EXTRA}?
        static readonly Regex RxBankT2 = new Regex(@";(?<pan>\d{4,19})=(?<rest>[^?]*)\?");

        // Track 2 PAN-only: ;{PAN}?
        static readonly Regex RxT2PanOnly = new Regex(@";(?<pan>\d{4,30})\?");

        // Generic: %...?\s*;...?
        static readonly Regex RxGenericTracks = new Regex(@"%(?<t1>[^?]*)\?\s*;(?<t2>[^?]*)\?");

        // Dạng “…?{digits}” (ví dụ: 2906521...?...4852...): lấy số sau dấu ?
        static readonly Regex RxPostQDigits = new Regex(@"\?(?<pan>\d{4,30})\s*$");

        // Barcode / nhập tay toàn số (không sentinel)
        static readonly Regex RxDigitsOnly = new Regex(@"^\s*(?<pan>\d{8,30})\s*$");

        /// <summary>
        /// Kiểm tra nhìn nhanh có “mùi” dữ liệu swipe không.
        /// </summary>
        public static bool IsSwipe(string swipeData)
        {
            if (string.IsNullOrEmpty(swipeData)) return false;
            return swipeData.IndexOf('%') >= 0
                || swipeData.IndexOf(';') >= 0
                || swipeData.IndexOf('^') >= 0
                || swipeData.IndexOf('?') >= 0;
        }

        /// <summary>
        /// Parse chi tiết, cố gắng nhận diện nhiều format.
        /// </summary>
        public static SwipeParseResult Parse(string input)
        {
            var res = new SwipeParseResult
            {
                Ok = false,
                CardType = "UNKNOWN",
                Format = SwipeFormat.Unknown,
                CardNumber = "",
                RawTrack1 = "",
                RawTrack2 = "",
                Error = ""
            };

            if (string.IsNullOrWhiteSpace(input))
            {
                res.Error = "Empty";
                return res;
            }

            // Chuẩn hóa: bỏ CR/LF/Tab, giữ lại khoảng trắng giữa
            var raw = input.Trim();
            raw = raw.Replace("\r", "").Replace("\n", "").Replace("\t", "");

            try
            {
                // 1) Gift: %GIFT-<ver>?;{PAN}?
                var mGift = RxGift.Match(raw);
                if (mGift.Success)
                {
                    res.Ok = true;
                    res.CardType = "GIFT";
                    res.Format = SwipeFormat.GiftV1;
                    res.FormatVersion = int.Parse(mGift.Groups["ver"].Value);
                    res.CardNumber = mGift.Groups["pan"].Value;
                    res.RawTrack1 = $"GIFT-{res.FormatVersion}";
                    res.RawTrack2 = res.CardNumber;
                    return res;
                }

                // 2) Bank Track 1
                var mT1 = RxBankT1.Match(raw);
                if (mT1.Success)
                {
                    res.Ok = true;
                    res.CardType = "BANK";
                    res.Format = SwipeFormat.BankTrack1;
                    res.CardNumber = mT1.Groups["pan"].Value;
                    res.RawTrack1 = mT1.Value;
                    return res;
                }

                // 3) Bank Track 2
                var mT2 = RxBankT2.Match(raw);
                if (mT2.Success)
                {
                    res.Ok = true;
                    res.CardType = "BANK";
                    res.Format = SwipeFormat.BankTrack2;
                    res.CardNumber = mT2.Groups["pan"].Value;
                    res.RawTrack2 = mT2.Value;
                    return res;
                }

                // 4) Track2 PAN-only ;{PAN}?
                var mT2Only = RxT2PanOnly.Match(raw);
                if (mT2Only.Success)
                {
                    res.Ok = true;
                    res.CardType = "UNKNOWN";
                    res.Format = SwipeFormat.PanOnlyTrack2;
                    res.CardNumber = mT2Only.Groups["pan"].Value;
                    res.RawTrack2 = mT2Only.Value;
                    return res;
                }

                // 5) Generic "%...?\s*;...?"
                var mGen = RxGenericTracks.Match(raw);
                if (mGen.Success)
                {
                    res.Ok = true;
                    res.Format = SwipeFormat.GenericTracks;
                    res.RawTrack1 = mGen.Groups["t1"].Value;
                    res.RawTrack2 = mGen.Groups["t2"].Value;

                    // Nếu Track2 toàn số thì coi như PAN
                    if (Regex.IsMatch(res.RawTrack2 ?? "", @"^\d{4,30}$"))
                    {
                        res.CardNumber = res.RawTrack2;
                        res.CardType = res.RawTrack1.StartsWith("GIFT-", StringComparison.OrdinalIgnoreCase) ? "GIFT" : "UNKNOWN";
                    }
                    else
                    {
                        // cố lấy số từ t2
                        var digits = Regex.Match(res.RawTrack2, @"\d{4,30}");
                        if (digits.Success) res.CardNumber = digits.Value;
                    }
                    return res;
                }

                // 6) Dạng “…?{digits}”
                var mAfterQ = RxPostQDigits.Match(raw);
                if (mAfterQ.Success)
                {
                    res.Ok = true;
                    res.Format = SwipeFormat.PostQuestionDigits;
                    res.CardNumber = mAfterQ.Groups["pan"].Value;
                    return res;
                }

                // 7) Toàn số (barcode/nhập tay)
                var mDigits = RxDigitsOnly.Match(raw);
                if (mDigits.Success)
                {
                    res.Ok = true;
                    res.Format = SwipeFormat.DigitsOnly;
                    res.CardNumber = mDigits.Groups["pan"].Value;
                    return res;
                }

                res.Error = "Unrecognized format";
                return res;
            }
            catch (Exception ex)
            {
                res.Ok = false;
                res.Error = ex.Message;
                return res;
            }
        }

        /// <summary>
        /// API cũ: rút gọn chỉ trả về CardNumber (PAN). Nếu không parse được, trả về nguyên input như hiện tại.
        /// </summary>
        public static string ExtractCardNumber(string swipeData)
        {
            try
            {
                var p = Parse(swipeData);
                if (p.Ok && !string.IsNullOrEmpty(p.CardNumber))
                    return p.CardNumber;

                // Giữ hành vi cũ: fallback trả về chuỗi gốc
                return swipeData;
            }
            catch (Exception ex)
            {
                return "Extract Error: " + ex.Message;
            }
        }
    }
}
