import React, {useEffect} from "react";
// @ts-ignore
import image2 from "/src/resources/Company_Logo.png";
import {LogOut, useAuth} from "/src/components/imports.ts";
import {BalanceAtom} from "/src/atoms";
import {useAtom} from "jotai/index";


export default function NavigationBar({ children }) {
    const authLogin = useAuth();
    const [balance, setBalance] = useAtom(BalanceAtom);
  useEffect(()=> {
       let user =authLogin.user;
       setBalance({balanceValue:user?.balanceValue!});
  },[] )
    return (
        <div className="navbar bg-base-100 h-16 min-h-[4rem] flex justify-between items-center px-5">
            {/* Logo */}
            <div className="ml-5 mt-5">
                <img
                    src={image2}
                    alt="logo"
                    style={{width: "70px", height: "70px"}}
                />
            </div>

            {/* Navigation Items */}
            <div className="nav-items flex space-x-6">
                {children}
            </div>

            {/* Balance and Logout */}
            {balance != null && (
                <div className="flex items-center space-x-4">
                    <span className="text-black font-bold">{balance.balanceValue}</span>
                    <button
                        className="relative group text-black text-lg font-semibold transition-all duration-300 cursor-pointer"
                    >
                        <LogOut></LogOut>
                        <span
                            className="absolute -bottom-1 left-0 w-full h-0.5 bg-gradient-to-r from-gray-500 to-red-500 scale-x-0 group-hover:scale-x-100 origin-left transition-transform duration-300"
                        ></span>
                    </button>
                </div>
            )}
        </div>
    );
}
