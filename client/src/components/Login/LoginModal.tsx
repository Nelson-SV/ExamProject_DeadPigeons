import React, {useEffect, useState} from "react";
import toast from "react-hot-toast";
import {PasswordInput, useAuth, UserNameInput} from "/src/components/imports.ts";
import {ErrorMessages, getUserInfoFromToken, ValidationError} from "/src/helpers";
import {useNavigate} from "react-router-dom";


interface LoginParams {
    isOpen: boolean
    setIsOpen: () => void
}

export const LoginModal = ({isOpen, setIsOpen}: LoginParams) => {
    const navigate = useNavigate();
    const authLogin = useAuth();
    const [modalClass, setModalClass] = useState("");
    const [serverErrors, setServerErrors] = useState<ValidationError | null>(null);
    const [serverErrorMessages, setServerErrorMessages] = useState<string[]>([]);
    const initialState = {
        password: "",
        email: "",
        errors: {
            password: "",
            email: ""
        },
        required: {
            email: false,
            password: false,
        }, loading: false
    }
    const [modalState, setModalState] = useState(initialState);

    useEffect(() => {
        setModalClass(isOpen ? "modal-open" : "");
        if (serverErrors) {
            const serverMessages: string[] = [];
            Object.entries(serverErrors).forEach(([field, messages]) => {
                if (messages) {
                    messages.forEach((message) => {
                        if (message) {
                            serverMessages.push(message);
                            toast.error(message);
                        }
                    });
                }
            });
            setServerErrorMessages(serverMessages);
        }
    }, [serverErrors, isOpen]);


    const closeModal = () => {

        setModalClass(isOpen ? "modal-open" : "");
        setModalState(initialState);
        setServerErrors(null);
        setServerErrorMessages([])
        setIsOpen();
    }

    const setPassword = (currentPass: string) => {
        setModalState((prev) => ({
            ...prev,
            password: currentPass,
            errors: {...prev.errors, password: ""},
            required: {...prev.required, password: false}
        }))
    }

    const setUserName = (userName: string) => {
        setModalState((prev) => ({
            ...prev,
            email: userName,
            errors: {...prev.errors, email: ""},
            required: {...prev.required, email: false}
        }))
    }

    const isInputEmpty = (input: string) => {
        return input.length === 0;
    }

    const loginUser = () => {
        if (isInputEmpty(modalState.password)) {
            setModalState((prev) => ({
                ...prev,
                errors: {...prev.errors, password: ErrorMessages.InvalidPassword},
                required: {...prev.required, password: true}
            }))
            return;
        }

        if (isInputEmpty(modalState.email)) {
            setModalState((prev) => ({
                ...prev,
                errors: {...prev.errors, email: ErrorMessages.EmailInvalid},
                required: {...prev.required, email: true}
            }))
            return;
        }

        setModalState((prev) => (
            {...prev, loading: true}));
        authLogin
            .login({email: modalState.email, password: modalState.password})
            .then((r) => {
                const userRole = getUserInfoFromToken().role;
                if (userRole.toLowerCase() == "admin") {
                    navigate("/admin/overview", {replace: true})
                    return;
                } else {
                    navigate("/user/play", {replace: true})
                }
            })
            .catch((e) => {
                setModalState((prev) => ({
                    ...prev,
                    loading: false,
                }));
                const errorData = e.response?.data;
                if (errorData?.errors) {
                    setServerErrors(errorData.errors);
                } else if (errorData?.message) {
                    toast.error(errorData.message);
                } else {
                    toast.error("Network error. Please try again later.");
                }
            });


    }

    return (
        <dialog className={`modal ${modalClass}`}>
            <div className="modal-box">
                <div>
                    <button onClick={closeModal} className="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">✕
                    </button>
                </div>
                <div className={"flex flex-col gap-2 mt-2"}>
                    <UserNameInput getInputValue={setUserName} value={modalState.email}
                                   placeholder={"Email"}></UserNameInput>
                    <PasswordInput placeholder={"Password"} title={""} getPassword={setPassword} value={modalState.password}></PasswordInput>
                </div>
                <div className={"flex flex-row justify-end mt-2"}>
                    {modalState.loading ? (
                        <div className={"mr-2"}>
                            <p>Loading...</p>
                        </div>
                    ) : (
                        <div className="mr-2">
                            <p className={`${modalState.required.email ? "text-red-500" : "text-transparent"} `}>
                                {modalState.errors.email}
                            </p>
                            <p className={`${modalState.required.password ? "text-red-500" : "text-transparent"}`}>
                                {modalState.errors.password}
                            </p>
                            <p className={`${serverErrorMessages.length > 0 ? "text-red-500" : "text-transparent"}`}>
                                {serverErrorMessages}
                            </p>
                        </div>
                    )}
                    <button onClick={loginUser}
                            className="w-36 h-15 text-white py-2 bg-red-600 font-semibold rounded-md border border-transparent hover:bg-black hover:text-white hover:border-black transition-colors duration-300 text-center">
                        Log ind
                    </button>
                </div>
            </div>
        </dialog>
    )
}