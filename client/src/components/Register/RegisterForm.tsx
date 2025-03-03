import {useState} from "react";
import {useValidatePassword} from "/src/helpers/ValidatePassword.ts";
import {ErrorMessages, ErrorResponse, ValidationError} from "/src/helpers";
import {http} from "/src/helpers";
import {AxiosResponse} from "axios";
import {ResetPasswordRequest, ResetPasswordResponse} from "/src/Api.ts";
import toast from "react-hot-toast";
import {useNavigate, useSearchParams} from "react-router-dom";
import {PasswordInput} from "../../components/imports.ts";

export const RegisterForm = () => {
    const navigate = useNavigate();
    const [searchParams,setSearchParams] = useSearchParams();

    const [serverErrors, setServerErrors] = useState<ValidationError | null>(null);
    const [registerState, setRegisterState] = useState({
        password: "",
        confirmedPassword: "",
        requiredConfirmed: false,
        requiredPassword: false,
        passwordError: "",
    })

    const setPassword = (currentPass: string) => {
        setRegisterState((prev) => ({
            ...prev,
            password: currentPass,
            passwordError: ""
        }))
    }

    const setConfirmedPassword = (currentPass: string) => {
        setRegisterState((prev) => ({
            ...prev,
            confirmedPassword: currentPass,
            passwordError: ""
        }))
    }

    const submitPassword = (pass: string, confirmed: string) => {
        if (pass.length == 0) {
            setRegisterState((prev) => ({
                ...prev,
                requiredPassword: true,
                passwordError: ErrorMessages.PasswordEmpty
            }))
            return;
        }
        if (confirmed != pass) {
            setRegisterState((prev) => ({
                ...prev,
                requiredPassword: true,
                passwordError: ErrorMessages.PasswordsMustMatch
            }))
            return;
        }

        if (!useValidatePassword.validateLength) {
            setRegisterState((prev) => ({
                ...prev,
                requiredPassword: true,
                passwordError: ErrorMessages.MinLength
            }))
            return;
        }


        if (!useValidatePassword.validateUpperCase(pass)) {
            setRegisterState((prev) => ({
                ...prev,
                requiredPassword: true,
                passwordError: ErrorMessages.UpperCharacterContain
            }))
            return;
        }

        if (!useValidatePassword.validateLowerCase(pass)) {
            setRegisterState((prev) => ({
                ...prev,
                requiredPassword: true,
                passwordError: ErrorMessages.LowerCaseContain
            }))
            return;
        }
        if (!useValidatePassword.validateHasNumber(pass)) {
            setRegisterState((prev) => ({
                ...prev,
                requiredPassword: true,
                passwordError: ErrorMessages.DigitContain
            }))
            return;
        }

        if (!useValidatePassword.validateSpecialCharacter(pass)) {
            setRegisterState((prev) => ({
                ...prev,
                requiredPassword: true,
                passwordError: ErrorMessages.NonAlphaNumeric
            }))
            return;
        }

        if (!useValidatePassword.validatePassword(pass)) {
            setRegisterState((prev) => ({
                ...prev,
                requiredPassword: true,
                passwordError: ErrorMessages.InvalidPassword
            }))
            return;
        }

        setRegisterState((prev) => ({
            ...prev,
            requiredPassword: false,
            passwordError: ""
        }))


        const formData: ResetPasswordRequest = {
            Email: searchParams.get("email") || "",
            Token: searchParams.get("token") || "",
            Password: registerState.password
        }
        console.log(formData);
        http.api.resetPassword(formData)
            .then((r: AxiosResponse<ResetPasswordResponse>) => {
                if (r.status === 201) {
                    toast.success(r.data.message)
                    setTimeout(() => {
                        navigate("/")
                    }, 2000)
                }
            }).catch((e) => {
            const errorData = e.response?.data;
            if (errorData?.errors) {
                const errors = errorData.errors as ErrorResponse;
                setServerErrors(errors.errors);
                if (errorData.errors) {
                    setServerErrors(errorData.errors);
                } else {
                    toast.error("An unexpected error occurred.");
                }
            } else {
                toast.error("Network error. Please try again later.");
            }
            {

                if(serverErrors){
                    Object.entries(serverErrors!).forEach(([field, messages]) => {
                        if (messages) {
                            messages.forEach((message) => {
                                if (message) {
                                    toast.error(message);
                                }
                            });
                        }
                    });
                }

            }
        });
    }

    return (
        <div className={"flex flex-col justify-center items-center mt-5  md:w-1/2 lg:w-1/3 xl:w-1/4 "}>
            <PasswordInput placeholder={"password"} title={"Password"} getPassword={setPassword}
                           value={registerState.password}></PasswordInput>
            <PasswordInput placeholder={"confirm password"} title={"Password confirmation"}
                           getPassword={setConfirmedPassword} value={registerState.confirmedPassword}></PasswordInput>
            <p className={`block text-center ${registerState.requiredPassword ? "text-red-500" : "text-transparent"}`}>
                {registerState.passwordError}
            </p>
            <button onClick={() => submitPassword(registerState.password, registerState.confirmedPassword)}
                    className="w-36 h-15 text-white py-2 bg-black font-semibold rounded-md border border-transparent hover:bg-red-500 hover:text-white hover:border-black transition-colors duration-300 text-center mt-5">
                Indsende
            </button>
            <div className="mt-6 p-4 bg-gray-100 border-l-4 border-blue-500 rounded shadow-sm">
                <h2 className="text-blue-700 font-bold text-lg mb-2">Password Requirements</h2>
                <p className="text-gray-700 text-sm">
                    Ensure your password meets the following criteria:
                </p>
                <ul className="list-disc pl-5 text-gray-600 mt-2 text-sm">
                    <li>At least 6 characters</li>
                    <li>At least one uppercase letter</li>
                    <li>At least one lowercase letter</li>
                    <li>At least one digit</li>
                    <li>At least one special character (e.g., @, #, !)</li>
                </ul>
            </div>
        </div>

    )
}