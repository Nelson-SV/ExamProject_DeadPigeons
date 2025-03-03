export interface useValidatePassword {
    validatePassword: (password: string) => boolean;
    validateLength: (password: string) => boolean;
    validateUpperCase: (password: string) => boolean;
    validateLowerCase: (password: string) => boolean;
    validateHasNumber: (password: string) => boolean;
    validateSpecialCharacter: (password: string) => boolean;
}

export const useValidatePassword: useValidatePassword = {
    validateLength: (password: string) => {
        const minLength = 6;
        return password.length >= minLength;
    },

    validateUpperCase: (password: string) => {
        return /[A-Z]/.test(password);
    },

    validateLowerCase: (password: string) => {
        return /[a-z]/.test(password);
    },

    validateHasNumber: (password: string) => {
        return /\d/.test(password);
    },

    validateSpecialCharacter: (password: string) => {
        return /[!@#$%^&*(),.?":{}|<>]/.test(password);
    },

    validatePassword: (password: string) => {
        return (
            useValidatePassword.validateLength(password) &&
            useValidatePassword.validateUpperCase(password) &&
            useValidatePassword.validateLowerCase(password) &&
            useValidatePassword.validateHasNumber(password) &&
            useValidatePassword.validateSpecialCharacter(password)
        );
    },
};
