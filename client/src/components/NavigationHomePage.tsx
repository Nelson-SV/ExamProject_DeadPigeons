import ThemeSwitcher from "./ThemeSwitcher.tsx";
// @ts-ignore
import image2 from "/src/resources/Company_Logo.png";
import React from "react";


export default function NavigationHomePage() {
    return (
        <div className="navbar bg-base-100 h-16 min-h-[4rem] ">
            <div className='ml-5 mt-5'>
                <img
                    src={image2}
                    alt="logo"
                    style={{width: '70px', height: '70px'}}
                />
            </div>
        </div>
    );
}