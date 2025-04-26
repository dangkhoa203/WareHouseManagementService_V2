import Container from "@mui/material/Container";
import {
    Checkbox,
    FormControl,
    Grid2,
    Input,
    InputAdornment,
    InputLabel, Paper,
    TextField
} from "@mui/material";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import IconButton from "@mui/material/IconButton";
import {Visibility, VisibilityOff} from "@mui/icons-material";
import {useState} from "react";

export default function Login(){
    const [showPassword, setShowPassword] = useState(false);

    const handleClickShowPassword = () => setShowPassword((show) => !show);

    const handleMouseDownPassword = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
    };

    const handleMouseUpPassword = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
    };
    return (
        <Container maxWidth="lg" sx={{display:"flex",justifyContent:"center",paddingTop:"30px"}} >
            <Paper elevation={12}
                   property="div"
                     sx={{
                border:"3px solid orange",
                minHeight:"400px",
                width:"500px",
                paddingY:"10px",
                display:"flex",justifyContent:"space-between",flexDirection:"column"}}  >
                <Typography fontSize={"3.5em"} textAlign={"center"}>
                    Đăng nhập
                </Typography>
                <Grid2 container spacing={2} paddingX="50px" >
                    <Grid2 sx={{textAlign:'center'}} size={12}>
                        <TextField color="warning" sx={{fontSize:"1.5em",width:"100%"}} label="UserName" variant="standard" />
                    </Grid2>
                    <Grid2 sx={{textAlign:'center'}} size={12}>
                        <FormControl fullWidth variant="standard">
                            <InputLabel color="warning" htmlFor="standard-adornment-password">Password</InputLabel>
                            <Input
                                color="warning"
                                id="standard-adornment-password"
                                type={showPassword ? 'text' : 'password'}
                                endAdornment={
                                    <InputAdornment position="end">
                                        <IconButton
                                            aria-label={
                                                showPassword ? 'hide the password' : 'display the password'
                                            }
                                            onClick={handleClickShowPassword}
                                            onMouseDown={handleMouseDownPassword}
                                            onMouseUp={handleMouseUpPassword}
                                        >
                                            {!showPassword ? <VisibilityOff className="password-icon" /> : <Visibility />}
                                        </IconButton>
                                    </InputAdornment>
                                }
                            />
                        </FormControl>
                    </Grid2>
                    <Grid2 sx={{textAlign:'start'}} size={6}>
                        <Checkbox color={"warning"} sx={{marginLeft:0,color:"orange"}}/> Test
                    </Grid2>
                </Grid2>
                <Button sx={{marginX:"100px"}} variant="contained">Contained</Button>
            </Paper>
        </Container>
    )
}