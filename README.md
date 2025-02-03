# Visualization System

## Overview

VisualizationSystem is a data analysis application that allows to cluster and visualize multi-parameter data using graphs and plots.
The application provides flexible configuration options for data analysis, clustering and visualization.

## Key Features

- Data Import: Supports importing Excel files with automatic detection of the data structure.
- Multi-tab Interface: Each data view or visualization opens in its own tab.
- Advanced Comparison Algorithm: Compares objects based on numerical and categorical parameters, allowing multiple values per cell (separated by commas).
- Flexible Similarity Settings: Allows to adjust comparison thresholds, set parameter weights, and enable/disable parameters.
- Multiple Clustering Algorithms: Implemented clustering methods include:
  - K-means.
  - DBSCAN.
  - Hierarchical Agglomerative Clustering (HAC).

## Visualization

### All Types of Graph Features
- Builds connections between nodes using full pairwise comparison algorithm.
- Configurable similarity thresholds.
- Edges colored by similarity percentage (green for high similarity, red for low).
- Double-click node to view detailed object information. Provides comprehensive view of object parameters and comparison results.

### Comparison Graph Features
- Node color intensity reflects number of connections.
- No connection created if similarity is below threshold.
![VisualizationSystem_GrxGdV6d81](https://github.com/user-attachments/assets/b5e32def-7a15-4579-b7e0-9b756a5bef14)

### Clustered Graph Features
- Nodes colored by cluster membership.
- Connections show strongest relationships between objects (can be below threshold).
![VisualizationSystem_BaqMwbHo6Q](https://github.com/user-attachments/assets/fd9c210a-d5b7-4db7-ba33-a537bfee7dc2)

### Additional Visualization Methods
- Scatter Plots: 
  - Uses PCA (Principal Component Analysis) to reduce multi-dimensional data to 2D representation.
  - Each point represents an object.
  - Color indicates cluster membership.
![VisualizationSystem_Cbj98B7B9H](https://github.com/user-attachments/assets/dcc855c7-961b-4a9b-8502-96424f559199)

- Cluster Blocks:
  - Visual grouping of objects by cluster.
  - Shows cluster membership without preserving spatial relationships.
![VisualizationSystem_bjJATYoQNq](https://github.com/user-attachments/assets/06a76d6f-821e-4f1b-8ce8-5b8fb447f12c)

## Clustering

### Clustering Algorithms
Custom-realization with the following features:
- Data normalization using MinMax method for numerical data
- One-Hot encoding for categorical data
- Customizable algorithm parameters:
  - K-means: 
    - Number of clusters,
    - Maximum iterations.
  - DBSCAN:
    - Epsilon (maximum distance between points),
    - Minimum points for cluster formation.
  - Hierarchical Agglomerative Clustering (HAC):
    - Merge threshold.
- Support for multiple distance metrics:
  - Euclidean distance for numeric.
  - Hamming distance for categorical.

## External Libraries

The application leverages several external libraries to enhance data visualization and processing:
- [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader): Excel file parsing.
- [Microsoft MSAGL](https://github.com/microsoft/automatic-graph-layout): Graph visualization.
- [ScottPlot](https://scottplot.net/): Scatter plot rendering.
- [ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet): Using PCA method.

## Example Data Source

The application is perfectly suited for analyzing data from the Fragile States Index (https://fragilestatesindex.org/excel/).
These datasets provide rich, multi-parameter information that can be effectively processed and visualized using application.

## Application Usage

### Database
- Provider: MS SQL Server
- Default database name: `node_objects_db`
- Connection string configuration required for non-local server instances

### Application Interfaces
- Data Menu:
  - "Load Excel": Import data from Excel file.
  - "View data": Open tab with table view of the current dataset.
  - "Datasets": Select or delete any loaded dataset.
- Visualization Menu:
  - "Build graph": Open tab with generated graph based on current settings.
  - "Build plot": Open tab with generated plot based on current settings.
  - "Settings": Open settings menu.
