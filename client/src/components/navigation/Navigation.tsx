import React from "react";
import NavigationBar from "./NavigationBar";
import { GradientNavLink } from "./GradientNavLink.tsx";

export default function Navigation({ links }) {
    return (
        <>
            <NavigationBar>
                {links.map(({ to, label }) => (
                    <GradientNavLink key={to} to={to}>
                        {label}
                    </GradientNavLink>
                ))}
            </NavigationBar>
        </>
    );
}
