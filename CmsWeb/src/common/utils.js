/* Utility Functions */

export const utils = {
    validateEmail(email) {
        if (!email || email.length < 6) return false;
        let regex = /^([a-zA-Z0-9_.+-])+@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return regex.test(email);
    }
}
