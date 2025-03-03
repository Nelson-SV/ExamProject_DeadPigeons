import React from "react";
import {useAuth} from "/src/hooks";
import {useLocation} from "react-router-dom";

export const LogOut = () => {
    const logger = useAuth()
    const location = useLocation()


    const logout = () => {
        if (location.pathname.startsWith("/admin")) {
            logger.logoutAdmin();
        } else {
            logger.logout();
        }

    }

    return (
        <span
            onClick={logout}
            className="group-hover:text-transparent group-hover:bg-clip-text group-hover:bg-gradient-to-r
                            group-hover:from-gray-500 group-hover:to-red-500"
        >
                            Log ud
                        </span>
    )

}