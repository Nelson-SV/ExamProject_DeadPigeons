import {Route, Routes} from "react-router-dom";
import {Toaster} from "react-hot-toast";
import {DevTools} from "jotai-devtools";
import {useAtom} from "jotai";
import Home from "../pages/Home.tsx";
import NavigationHomePage from "./NavigationHomePage.tsx";
import Footer from "/src/components/shared/Footer.tsx";
import AdminNavigation from "/src/components/navigation/AdminNavigation.tsx";
import UserNavigation from "/src/components/navigation/UserNavigation.tsx";
import {RegisterPage} from "/src/pages/Register.tsx";
import {getUserInfoFromToken} from "/src/helpers";
import {jwtAtom} from "/src/hooks/auth.ts";
import {useEffect, useState} from "react";


const App = () => {

    const [getToken, _] = useAtom(jwtAtom);
    const [getRole, setRole] = useState<string | null>(null);

    useEffect(() => {
        if (getToken !== null) {
            let roleTemp = getUserInfoFromToken().role;
            setRole(roleTemp);
        }
    }, [getToken]);


    return (<>
            <div className="flex flex-col min-h-screen">
                <Toaster position="bottom-center"/>
                <div className="flex-grow">
                    <Routes>
                        <Route
                            path="/"
                            element={
                                <>
                                    <NavigationHomePage/>
                                    <Home/>
                                </>
                            }
                        />
                        {getRole === "Player" ? (
                            <>
                                <Route
                                    path="/user/*"
                                    element={<UserNavigation/>}
                                />
                            </>
                        ) : (
                            <>
                                <Route
                                    path="/admin/*"
                                    element={<AdminNavigation/>}
                                />
                            </>
                        )}
                        <Route path={"/password-reset/"} element={<RegisterPage/>}/>
                    </Routes>
                </div>
                <Footer/>
                <DevTools/>
            </div>
        </>
    )
}

export default App;