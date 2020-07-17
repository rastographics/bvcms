/* Utility Functions */

export const utils = {

    validateEmail(email) {
        if (!email || email.length < 6) return false;
        let regex = /^([a-zA-Z0-9_.+-])+@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return regex.test(email);
    },

    cardType(number) {
        // Visa
        var re = new RegExp("^4");
        if (number.match(re) != null) {
            return "visa";
        }

        // Mastercard
        re = new RegExp("^5[1-5]");
        if (number.match(re) != null) {
            return "mastercard";
        }

        // AMEX
        re = new RegExp("^3[47]");
        if (number.match(re) != null) {
            return "amex";
        }

        // Discover
        re = new RegExp("^(6011|622(12[6-9]|1[3-9][0-9]|[2-8][0-9]{2}|9[0-1][0-9]|92[0-5]|64[4-9])|65)");
        if (number.match(re) != null) {
            return "discover";
        }

        return "other";
    },

    getOrdinal(n) {
        var s = ["th", "st", "nd", "rd"], v = n % 100;
        return n + (s[(v - 20) % 10] || s[v] || s[0]);
    }
}
