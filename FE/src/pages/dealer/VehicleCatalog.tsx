import React, { useState } from 'react';
import {
  Grid,
  Card,
  CardContent,
  CardMedia,
  Typography,
  Box,
  Chip,
  Button,
  TextField,
  InputAdornment,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
} from '@mui/material';
import {
  Search,
  Compare,
  Visibility,
} from '@mui/icons-material';
import { useQuery } from '@tanstack/react-query';
import Layout from '../../components/layout/Layout';
import { vehicleApi } from '../../services/api';

const VehicleCatalog: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedModel, setSelectedModel] = useState('');
  const [selectedPriceRange, setSelectedPriceRange] = useState('');
  const [selectedVehicles, setSelectedVehicles] = useState<string[]>([]);

  const { data: vehiclesResponse, isLoading } = useQuery({
    queryKey: ['vehicles'],
    queryFn: () => vehicleApi.getVehicles(),
  });

  const vehicles = vehiclesResponse?.data || [];

  const filteredVehicles = vehicles.filter(vehicle => {
    const matchesSearch = vehicle.model.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         vehicle.version.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesModel = !selectedModel || vehicle.model === selectedModel;
    const matchesPrice = !selectedPriceRange || checkPriceRange(vehicle.basePrice, selectedPriceRange);
    
    return matchesSearch && matchesModel && matchesPrice;
  });

  const checkPriceRange = (price: number, range: string) => {
    switch (range) {
      case 'under-50k':
        return price < 50000;
      case '50k-70k':
        return price >= 50000 && price < 70000;
      case '70k-100k':
        return price >= 70000 && price < 100000;
      case 'over-100k':
        return price >= 100000;
      default:
        return true;
    }
  };

  const handleCompareToggle = (vehicleId: string) => {
    setSelectedVehicles(prev => 
      prev.includes(vehicleId)
        ? prev.filter(id => id !== vehicleId)
        : [...prev, vehicleId]
    );
  };

  const uniqueModels = [...new Set(vehicles.map(v => v.model))];

  return (
    <Layout title="Vehicle Catalog">
      <Box sx={{ mb: 3 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Vehicle Catalog
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Browse our complete selection of electric vehicles
        </Typography>
      </Box>

      {/* Filters */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Grid container spacing={2} alignItems="center">
            <Grid item xs={12} md={4}>
              <TextField
                fullWidth
                placeholder="Search vehicles..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <Search />
                    </InputAdornment>
                  ),
                }}
              />
            </Grid>
            <Grid item xs={12} md={3}>
              <FormControl fullWidth>
                <InputLabel>Model</InputLabel>
                <Select
                  value={selectedModel}
                  onChange={(e) => setSelectedModel(e.target.value)}
                  label="Model"
                >
                  <MenuItem value="">All Models</MenuItem>
                  {uniqueModels.map(model => (
                    <MenuItem key={model} value={model}>{model}</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} md={3}>
              <FormControl fullWidth>
                <InputLabel>Price Range</InputLabel>
                <Select
                  value={selectedPriceRange}
                  onChange={(e) => setSelectedPriceRange(e.target.value)}
                  label="Price Range"
                >
                  <MenuItem value="">All Prices</MenuItem>
                  <MenuItem value="under-50k">Under $50K</MenuItem>
                  <MenuItem value="50k-70k">$50K - $70K</MenuItem>
                  <MenuItem value="70k-100k">$70K - $100K</MenuItem>
                  <MenuItem value="over-100k">Over $100K</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} md={2}>
              <Button
                fullWidth
                variant="contained"
                startIcon={<Compare />}
                disabled={selectedVehicles.length < 2}
                onClick={() => {/* Navigate to comparison page */}}
              >
                Compare ({selectedVehicles.length})
              </Button>
            </Grid>
          </Grid>
        </CardContent>
      </Card>

      {/* Vehicle Grid */}
      <Grid container spacing={3}>
        {filteredVehicles.map((vehicle) => (
          <Grid item xs={12} sm={6} md={4} key={vehicle.id}>
            <Card
              sx={{
                height: '100%',
                display: 'flex',
                flexDirection: 'column',
                transition: 'transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out',
                '&:hover': {
                  transform: 'translateY(-4px)',
                  boxShadow: 4,
                },
              }}
            >
              <CardMedia
                component="img"
                height="200"
                image={vehicle.images[0] || '/placeholder-vehicle.jpg'}
                alt={vehicle.model}
                sx={{ objectFit: 'cover' }}
              />
              <CardContent sx={{ flexGrow: 1, display: 'flex', flexDirection: 'column' }}>
                <Typography variant="h6" component="h2" gutterBottom>
                  {vehicle.model}
                </Typography>
                <Typography variant="subtitle1" color="text.secondary" gutterBottom>
                  {vehicle.version}
                </Typography>
                
                <Box sx={{ mb: 2 }}>
                  <Chip
                    label={vehicle.color}
                    size="small"
                    variant="outlined"
                    sx={{ mr: 1, mb: 1 }}
                  />
                  <Chip
                    label={`${vehicle.year}`}
                    size="small"
                    variant="outlined"
                    sx={{ mr: 1, mb: 1 }}
                  />
                  <Chip
                    label={`Stock: ${vehicle.stock}`}
                    size="small"
                    color={vehicle.stock > 5 ? 'success' : vehicle.stock > 0 ? 'warning' : 'error'}
                    sx={{ mb: 1 }}
                  />
                </Box>

                <Typography variant="h5" color="primary" sx={{ mb: 2 }}>
                  ${vehicle.basePrice.toLocaleString()}
                </Typography>

                <Box sx={{ flexGrow: 1 }}>
                  <Typography variant="body2" color="text.secondary" gutterBottom>
                    Key Features:
                  </Typography>
                  <Box sx={{ mb: 2 }}>
                    {vehicle.features.slice(0, 3).map((feature, index) => (
                      <Chip
                        key={index}
                        label={feature}
                        size="small"
                        sx={{ mr: 0.5, mb: 0.5 }}
                      />
                    ))}
                    {vehicle.features.length > 3 && (
                      <Chip
                        label={`+${vehicle.features.length - 3} more`}
                        size="small"
                        variant="outlined"
                        sx={{ mr: 0.5, mb: 0.5 }}
                      />
                    )}
                  </Box>
                </Box>

                <Box sx={{ display: 'flex', gap: 1, mt: 'auto' }}>
                  <Button
                    variant="outlined"
                    startIcon={<Visibility />}
                    fullWidth
                    onClick={() => {/* Navigate to vehicle details */}}
                  >
                    View Details
                  </Button>
                  <Button
                    variant={selectedVehicles.includes(vehicle.id) ? 'contained' : 'outlined'}
                    onClick={() => handleCompareToggle(vehicle.id)}
                    disabled={selectedVehicles.length >= 3 && !selectedVehicles.includes(vehicle.id)}
                  >
                    {selectedVehicles.includes(vehicle.id) ? 'Selected' : 'Compare'}
                  </Button>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      {filteredVehicles.length === 0 && !isLoading && (
        <Box sx={{ textAlign: 'center', py: 4 }}>
          <Typography variant="h6" color="text.secondary">
            No vehicles found matching your criteria
          </Typography>
        </Box>
      )}
    </Layout>
  );
};

export default VehicleCatalog;
