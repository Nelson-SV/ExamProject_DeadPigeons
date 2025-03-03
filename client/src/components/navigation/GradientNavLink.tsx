import React from "react";
import { NavLink } from "react-router-dom";

export function GradientNavLink({ to, children }) {
    return (
        <NavLink
            to={to}
            className={({ isActive }) =>
                `relative group font-semibold text-center text-xl whitespace-nowrap px-3
                ${isActive ? 'text-transparent bg-clip-text bg-gradient-to-r from-gray-500 to-red-500' : 'text-black'}`
            }
        >
            <span
                className="group-hover:text-transparent group-hover:bg-clip-text group-hover:bg-gradient-to-r
                group-hover:from-gray-500 group-hover:to-red-500"
            >
                {children}
            </span>
            <span
                className="absolute -bottom-1 left-0 w-full h-0.5 bg-gradient-to-r from-gray-500 to-red-500 scale-x-0 group-hover:scale-x-100 origin-left transition-transform duration-300"
            ></span>
        </NavLink>
    );
}
