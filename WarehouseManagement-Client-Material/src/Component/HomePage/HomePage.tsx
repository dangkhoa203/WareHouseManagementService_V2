import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import MenuIcon from '@mui/icons-material/Menu';
import Container from '@mui/material/Container';
import Button from '@mui/material/Button';
import AdbIcon from '@mui/icons-material/Adb';
import {Link, matchPath, Outlet, useLocation, useNavigate} from "react-router";
import {Drawer, Menu, MenuItem, Tooltip} from "@mui/material";
import {useState} from "react";
import {AccountCircle} from "@mui/icons-material";
import userInfoFunction from "../../Type/userInfoFunction.tsx";
import {useUserInfo} from "../../State/User.ts";


export default function HomePage() {

    const navigate = useNavigate();
    const userInfo = useUserInfo((state)=> state.user);
    const [open, setOpen] = useState(false);
    const { pathname } = useLocation();
    const isLoginPath = matchPath("/Login", pathname);
    const isRegisterPath = matchPath("/Register", pathname);
    const toggleDrawer = (newOpen: boolean) => () => {
        setOpen(newOpen);
    };

    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

    const handleMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };
    const [logOutLoading, setLogOutLoading] = useState(false);
    const logOut=async ()=>{
        // try{
        //     setLogOutLoading(true);
        //     await fetch('https://localhost:7075/api/Account/LogOut', {
        //         method: 'POST',
        //         headers: { 'Content-Type': 'application/json' },
        //         credentials: 'include',
        //     });
        //     await props.getInfo()
        // }catch{
        //     console.log("Error")
        // }finally {
        //     setLogOutLoading(false);
        // }
    }
    return (
        <>
            <AppBar id={"Home-NavBar"} className={userInfo.isLoggedIn ? "Home-NavBar-LoggedIn":""} position="sticky">
                <Container maxWidth="xl">
                    <Toolbar disableGutters>
                        <AdbIcon sx={{ display: { xs: 'none', md: 'flex' }, mr: 1 }} />
                        <Typography
                            variant="h6"
                            noWrap
                            component="a"
                            href="/"
                            sx={{
                                mr: 2,
                                display: { xs: 'none', md: 'flex' },
                                fontFamily: 'monospace',
                                fontWeight: 700,
                                fontSize:'3em',
                                letterSpacing: '.3rem',
                                color: 'inherit',
                                textDecoration: 'none',
                            }}
                        >
                            LOGO
                        </Typography>

                        <Box sx={{ flexGrow: 1, display: { xs: 'flex', md: 'none' } }}>
                            <IconButton
                                size="large"
                                aria-controls="menu-appbar"
                                aria-haspopup="true"
                                onClick={toggleDrawer(true)}
                                color="inherit"
                            >
                                <MenuIcon />
                            </IconButton>
                            <Drawer disableScrollLock={true} anchor="left" open={open} onClose={toggleDrawer(false)}>
                                <Box sx={{ width: 250 }} role="presentation" onClick={toggleDrawer(false)}>
                                    test
                                </Box>
                            </Drawer>

                        </Box>
                        <AdbIcon sx={{ display: { xs: 'flex', md: 'none' }, mr: 1 }} />
                        <Typography
                            variant="h5"
                            noWrap
                            component="a"
                            href="#app-bar-with-responsive-menu"
                            sx={{
                                mr: 2,
                                display: { xs: 'flex', md: 'none' },
                                flexGrow: 1,
                                fontFamily: 'monospace',
                                fontWeight: 700,
                                letterSpacing: '.3rem',
                                color: 'inherit',
                                textDecoration: 'none',
                            }}
                        >
                            LOGO
                        </Typography>
                        <Box sx={{ flexGrow: 1,gap:"30px", display: { xs: 'none', md: 'flex' } }}>
                            <Link to="About">Về chúng tôi</Link>

                            <Link to="Function">Chức năng</Link>
                            {userInfo.isLoggedIn && <Link to="/Workspace">Workspace</Link>}

                        </Box>
                        <Box sx={{ flexGrow: 0 }}>
                            {userInfo.isLoggedIn ?
                                <>
                                    <div>
                                        <span style={{color:"white"}}>{userInfo.userName}</span>
                                        <Tooltip title="Open settings">
                                            <IconButton
                                                size="large"
                                                aria-controls="menu-appbar"
                                                aria-haspopup="true"
                                                onClick={handleMenu}
                                                color="inherit"

                                            >
                                                <AccountCircle  style={{color:"white",fontSize:"1.3em"}} />
                                            </IconButton>
                                        </Tooltip>
                                        <Menu
                                            disableScrollLock={true}
                                            id="menu-appbar"
                                            anchorEl={anchorEl}
                                            anchorOrigin={{
                                                vertical: 'top',
                                                horizontal: 'right',
                                            }}
                                            keepMounted
                                            transformOrigin={{
                                                vertical: 'top',
                                                horizontal: 'right',
                                            }}
                                            open={Boolean(anchorEl)}
                                            onClose={handleClose}
                                        >
                                            <MenuItem disabled={logOutLoading} onClick={async ()=>{
                                                handleClose();
                                                await logOut();
                                            }}>Log Out</MenuItem>
                                        </Menu>
                                    </div>
                                </>
                                :
                                <>
                                    <Button color="warning" onClick={()=>{navigate("/Login")}}  size="large">Đăng nhập</Button>
                                    <Button color="warning" onClick={()=>{navigate("/Register")}}  size="large">Đăng ký</Button>
                                </>
                            }

                        </Box>
                    </Toolbar>
                </Container>
            </AppBar>
            <Outlet />
            {!(isLoginPath || isRegisterPath) &&
                <Container maxWidth="xl"  id="Footer">
                    <Typography>
                        @2025 DKWebSoft
                    </Typography>
                </Container>
            }
        </>
    );
}
