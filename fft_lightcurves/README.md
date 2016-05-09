Description of the files:
1. mpc-fetch.py - taken from Minor Planet Center script which is used for collecting data from MPC database.
2. get_data.py - script is used for dumping an asteroid data in json format. Asteroids which are used have an uncertainty set in range [0,1]
3. get_neowise_data.py - script for obtaining spectral data of asteroids from NASA NEOWISE web server.
4. get_correl_in_data.py - check correlation of asteroid parameters.
5. get_lightcurve_data.py - prepare data for identification based of fft. Several constraints are applied for better analyze. Mainly - the observation lesser than 11 hours.
6. light_model.py - main procedure of identification. Still has some constraints.
