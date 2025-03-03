import React from "react";
import Navigation from "./Navigation";
import {Route, Routes} from "react-router-dom";
import Payments from "/src/components/AdminComponents/PaymentsPage/Payments.tsx";
import {SearchResults,AdminHistory,AdminMainPage} from "/src/pages";
import UserManagementPage from "/src/components/admin/UserManagement/UserManagementPage.tsx";

export default function AdminNavigation() {
    const adminLinks = [
        {to: "/admin/overview", label: "Overview"},
        {to: "/admin/history", label: "History"},
        {to: "/admin/users-management", label: "Users Management"},
        {to: "/admin/payments", label: "Payments"}
    ];

    return (
        <>
            <Navigation links={adminLinks}/>
            <Routes>
                <Route path="payments" element={<Payments/>}/>
                <Route path="overview" element={<AdminMainPage/>}/>
                <Route path="users-management" element={<UserManagementPage/>}/>
                <Route path={"history"} element={<AdminHistory/>}></Route>
                <Route path={"history/:gameId"} element={<SearchResults/>}></Route>
            </Routes>
        </>

    );
}
