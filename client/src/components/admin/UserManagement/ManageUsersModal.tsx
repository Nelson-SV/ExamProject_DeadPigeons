import React, { useState, useEffect } from "react";
import {UserNameInput, EmailInput, PhoneNumberInput} from "/src/components/imports.ts";

const validateEmail = (email: string) => /\S+@\S+\.\S+/.test(email);

export default function ManageUsersModal({ isOpen, onClose, onSave, mode, user }) {
    const [formData, setFormData] = useState({
        userId: "",
        name: "",
        email: "",
        phoneNumber: "",
        isActive: true,
        role:"player"
    });
    const [errors, setErrors] = useState({ name: "", email: "", phoneNumber: "" });

    useEffect(() => {
        if (isOpen) {
            if (mode === "edit" && user) {
                setFormData({
                    userId: user.userId,
                    name: user.name,
                    email: user.email,
                    phoneNumber: user.phoneNumber || "",
                    isActive: user.isActive,
                    role: "player"
                });
            } else {
                setFormData({userId: "", name: "", email: "", phoneNumber: "", isActive: true, role: "player"});
            }
        }
    }, [isOpen, mode, user]);

    const handleInputChange = (field, value) => {
        setFormData({ ...formData, [field]: value });
    };

    const validateForm = () => {
        const newErrors = { name: "", email: "", phoneNumber: "" };

        if (!formData.name) {
            newErrors.name = "Name is required.";
        }
        if (!formData.email || !validateEmail(formData.email)) {
            newErrors.email = "Invalid email.";
        }
        if (!formData.phoneNumber || formData.phoneNumber.length != 8) {
            newErrors.phoneNumber = "Phone must be numeric with 8 digits.";
        }

        setErrors(newErrors);

        return !newErrors.name && !newErrors.email && !newErrors.phoneNumber;
    };


    const handleSave = () => {
         if (validateForm()) {
            onSave(formData);
            onClose();
         }
    };

    const handleCancel = () => {
        setErrors({ name: "", email: "", phoneNumber: "" });
        onClose();
    };


    if (!isOpen) return null;

    return (
        <dialog className="modal bg-opacity-60 bg-black" open>
            <div className="modal-box">
                <h3 className="font-bold text-lg">{mode === "edit" ? "Rediger bruger" : "Tilføj ny bruger"}</h3>
                <div className="py-4">
                    <div className="mb-4">
                        <label>Navn:</label>
                        <UserNameInput getInputValue={(e) => handleInputChange("name", e.valueOf())}
                                       value={formData.name}
                                       placeholder={"Navn"}></UserNameInput>
                        {errors.name && <p className="text-red-500 text-sm">{errors.name}</p>}
                    </div>
                    <div className="mb-4">
                        <label>Email:</label>
                        <EmailInput
                            getInputValue={(value) => handleInputChange("email", value.valueOf())}
                            value={formData.email}
                            placeholder="Indtast e-mail"
                        />
                        {errors.email && <p className="text-red-500 text-sm">{errors.email}</p>}
                    </div>
                    <div className="mb-4">
                        <label>Mobilnummer:</label>
                        <PhoneNumberInput
                            getInputValue={(value) => handleInputChange("phoneNumber", value.valueOf())}
                            value={formData.phoneNumber}
                            placeholder="Indtast mobilnummer"
                        />
                        {errors.phoneNumber && <p className="text-red-500 text-sm">{errors.phoneNumber}</p>}
                    </div>
                    {mode === "edit" && (
                        <div className="mb-4 flex items-center">
                            <label>Aktiv:</label>
                            <input
                                type="checkbox"
                                className="toggle toggle-success focus:ring-2 ml-2"
                                defaultChecked={formData.isActive}
                                onChange={(e) => {
                                    handleInputChange("isActive", e.target.checked);
                                    e.target.blur();
                                }}
                            />
                        </div>
                    )}
                </div>
                <div className="modal-action">
                    <button
                        className="w-20 h-12 bg-blue-400 text-white py-2 font-semibold rounded-md border border-transparent hover:bg-blue-900 hover:text-white transition-colors duration-300 text-center"
                        onClick={handleSave}>
                        Gemme
                    </button>
                    <button
                        className="w-20 h-12 bg-red-600 text-white py-2 font-semibold rounded-md border border-transparent hover:bg-black hover:text-white transition-colors duration-300 text-center"
                        onClick={handleCancel}>
                        Afbryd
                    </button>
                </div>
            </div>
        </dialog>
    );
}
