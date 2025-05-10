using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Siemens.Net2025
{
    //The validator of a book
    class Validator
    {
        /*
         * This function validates the input as an id of a book
         * @returns : true (if the input is a valid integer > 0)
         *            false (otherwise)
         */
        public static bool ValidateId (string input)
        {
            if (int.TryParse(input, out int result))
            {
                return result > 0;
            }
            return false;
        }

        /*
         * This function validates the input as a title of a book
         * @returns : true (if the input contains only letters, numbers, spaces,
         *            basic punctuation and has a length >= 2)
         *            false (otherwise)
         */
        public static bool ValidateTitle(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            return Regex.IsMatch(input, @"^[a-zA-Z0-9\s\.,:;!\?'\-]{2,}$");
        }

        /*
         * This function validates the input as an author of a book
         * @returns : true (if the input contains only letters, spaces,
         *                  hyphens or apostrophes and has a length >= 2)
         *            false (otherwise)
         */
        public static bool ValidateAuthor(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            return Regex.IsMatch(input, @"^[a-zA-Z\s\-']{2,}$");
        }

        /*
         * This function validates the input as a quantity of a book
         * @returns : true (if the input is a valid integer > 0)
         *            false (otherwise)
         */
        public static bool ValidateQuantity(string input)
        {
            if(int.TryParse(input, out int result))
            {
                return result > 0;
            }
            return false;
        }
    }
}
