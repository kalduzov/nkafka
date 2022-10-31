# Setup dev environment

### Step 0 

Docker should already be installed in your environment.

### Step 1 (Only for Windows) 

Install CHOCOLATEY

 > If Chocolatey is already installed skip this step

Open powershell and run

```./choco/install.ps1 ```

### Step 2 

Setting up certificates

#### Windows
```./init.ssl.ps1 ```

#### Linux
```./init.ssl.sh ```

### Step 3. 

Deploy Kafka

#### Windows
```./deploy.ps1 ```

#### Linux
```./deploy.sh ```