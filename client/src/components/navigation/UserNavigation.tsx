import React from "react";
import Navigation from "./Navigation";
import {Route, Routes} from "react-router-dom";
import PlayPage from "../PlayPage/PlayPage.tsx";
import {BalancePage} from "/src/pages/BalancePage.tsx";
import HistoryPage from "/src/components/user/historyPage/HistoryPage.tsx";

export default function UserNavigation() {
    const userLinks = [
        { to: "/user/play", label: "Spil" },
        { to: "/user/history", label: "Historie" },
        { to: "/user/balance", label: "Tank op" }
    ];

    return (
        <>
            <Navigation links={userLinks} />
            <Routes>
                {/* sub-routes for /user/* */}
                <Route path="play" element={<PlayPage />} />
                <Route path="balance" element={<BalancePage/>} />
                <Route path="history" element={<HistoryPage/>} />
            </Routes>
        </>
    );
}
