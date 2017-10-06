from __future__ import print_function
from features import mfcc
from features import logfbank
#from kullbackLiebler import gau_js
import scipy.io.wavfile as wav
import numpy as np

# help function to printer or not to printer
printerOn = False
def printer(content, end=""):
    if (printerOn == True):
        print(content)
        print(end)
        
def kl_divergence (pm, pv, qm, qv):
    invQv = np.linalg.inv(qv)
    traceQvPv = np.trace(np.dot(invQv, pv))
    meanDiff = np.subtract(qm, pm)
    detPv = np.linalg.det(pv)
    detQv = np.linalg.det(qv)
    detln = np.log(detPv / detQv)
    d = 12
    dist = 0.5 * ( traceQvPv + np.dot(np.dot(meanDiff, invQv), meanDiff) - d - detln )
    return dist

def dist(pm, pv, qm, qv):
    return kl_divergence(pm, pv, qm, qv) + kl_divergence(qm, qv, pm, pv)

limit = 6000000
(rate,sig) = wav.read("..\\..\\testy\\file.wav")
#(rate,sig) = wav.read("file2.wav")
id2remove = np.arange(limit,sig.shape[0])
sig = np.delete(sig,id2remove,0)
mfcc_feat = mfcc(sig,rate,0.025,0.01,12) # 12 MFCC coefficients

# Mean vector
mean_vector = np.mean(np.array(mfcc_feat),axis=0)
printer("Mean vector")
#printer("[", end="")
for i in range(0, mean_vector.shape[0]-1):
    printer(mean_vector[i], end=",")
i=i+1
printer(mean_vector[i], end="")
printer("]")
printer("*")

# Covariance matrix
covariance_matrix = np.cov(np.array(mfcc_feat).T)
printer("Covariance matrix")
printer("[", end="")
for i in range(0, covariance_matrix.shape[0]-1):
    printer("[", end="")
    for j in range(0, covariance_matrix.shape[1]-1):
        printer(covariance_matrix[i,j], end=",")
    j=j+1
    printer(covariance_matrix[i,j], end="")
    printer("]")
printer("]", end="")

#(rate2,sig2) = wav.read("..\\..\\testy\\file2.wav")
(rate2,sig2) = wav.read("file.wav")
mfcc_feat2 = mfcc(sig2,rate2,0.025,0.01,12)
mean_vector2 = np.mean(np.array(mfcc_feat2),axis=0)
covariance_matrix2 = np.cov(np.array(mfcc_feat2).T)

# KL distance
pm = mean_vector2
pv = covariance_matrix2
qm = mean_vector
qv = covariance_matrix
print("KL distance")
klDistance = dist(pm,pv,qm,qv) #pm, pv, qm, qv
print(klDistance)
