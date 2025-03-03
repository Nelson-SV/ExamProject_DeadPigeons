import React, {useEffect, useState} from "react";


import {GameAnimation} from "../components/imports.ts";
import {LoginModal} from "/src/components/Login/LoginModal.tsx";

export default function Home() {
    const [openModal, setOpenModal] = useState(false);

    const openModalAction = () => {
        setOpenModal(true)
    }

    const closeModalAction = () => {
        setOpenModal(false);
    }
    return (
        <div> {/* Root container */}
            <div className="flex items-center flex-col gap-2 mt-6">
                <div>
                    <h1 className="font-sans text-black text-4.5xl font-medium">
                        Døde Duer
                    </h1>
                </div>
                <div>
                    <h2 className="text-4xl leading-loose font-semibold text-transparent bg-clip-text bg-gradient-to-r from-gray-500 to-red-500">
                        Spil og vind hver Søndag
                    </h2>
                </div>

                <GameAnimation></GameAnimation>
                <div className="mt-6">
                    <button
                        onClick={() => openModalAction()}
                        className="w-36 h-15 text-white py-2 bg-red-600 font-semibold rounded-md border border-transparent hover:bg-black hover:text-white hover:border-black transition-colors duration-300 text-center">
                        Log ind
                    </button>
                </div>
                <LoginModal isOpen={openModal} setIsOpen={closeModalAction}></LoginModal>
            </div>
        </div>
    );

}